﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if (ENABLE_INPUT_SYSTEM)
using UnityEngine.InputSystem;
#endif

namespace UniStorm.CharacterController
{
    //This script has been modified compared to the original version of the script found here: http://wiki.unity3d.com/index.php?title=RigidbodyFPSWalker
    //A run system has been added to allow for running.
    //Proper collision detection has also been added so user cannot continue to jump on and over objects.
    //The script has also been made more efficient by calling GetComponent on start, storing it in a variable, then calling that variable throught the script.
    //This script is based on the license CC BY-SA 3.0

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(AudioSource))]
    public class UniStormCharacterController : MonoBehaviour
    {
        public float walkSpeed = 6.0f;
        public float runSpeed = 12.0f;
        public float gravity = 10.0f;
        public float maxVelocityChange = 10.0f;
        public bool canJump = true;
        public float jumpHeight = 2.0f;
        public bool onlyJumpOnUntagged = true;

        public AudioClip footStepSound;
        public float runFootStepSeconds = 0.5f;
        public float walkFootStepSeconds = 0.75f;

        private float footStepTimer;
        private AudioSource audioSource;
        private bool grounded = false;
        private float rayDistance;
        private RaycastHit hit;
        private Rigidbody rb;

        private Vector3 velocity;
        private Vector3 velocityChange;

        float XValue;
        float YValue;


        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.useGravity = false;
        }

        void FixedUpdate()
        {
            if (grounded)
            {
#if (ENABLE_LEGACY_INPUT_MANAGER)
                LegacyInputSystem();
#elif (ENABLE_INPUT_SYSTEM)
                NewInputSystem();
#endif
            }

            // We apply gravity manually for more tuning control
            rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));

            grounded = false;
        }

#if (ENABLE_INPUT_SYSTEM)
        void NewInputSystem()
        {
            if (Keyboard.current.wKey.isPressed)
            {
                YValue = Mathf.Lerp(YValue, 1, Time.deltaTime * 100);
            }

            if(Keyboard.current.sKey.isPressed)
            {
                YValue = Mathf.Lerp(YValue, -1, Time.deltaTime * 100);
            }

            if(Keyboard.current.aKey.isPressed)
            {
                XValue = Mathf.Lerp(XValue, -1, Time.deltaTime * 100);
            }

            if(Keyboard.current.dKey.isPressed)
            {
                XValue = Mathf.Lerp(XValue, 1, Time.deltaTime * 100);
            }

            if (!Keyboard.current.wKey.isPressed && !Keyboard.current.sKey.isPressed && !Keyboard.current.aKey.isPressed && !Keyboard.current.dKey.isPressed)
            {
                XValue = Mathf.Lerp(XValue, 0, Time.deltaTime * 100);
                YValue = Mathf.Lerp(YValue, 0, Time.deltaTime * 100);
            }

            // Calculate how fast we should be moving while running
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                Vector3 targetVelocity = new Vector3(XValue, 0, YValue);
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= runSpeed;

                // Apply a force that attempts to reach our target velocity
                velocity = rb.velocity;
                velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                rb.AddForce(velocityChange, ForceMode.VelocityChange);

                if (Keyboard.current.wKey.isPressed)
                {
                    footStepTimer += Time.deltaTime;
                }

                if (footStepTimer >= runFootStepSeconds && audioSource != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(footStepSound);
                    footStepTimer = 0;
                }
            }

            // Calculate how fast we should be moving while walking
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                Vector3 targetVelocity = new Vector3(XValue, 0, YValue);
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= walkSpeed;

                // Apply a force that attempts to reach our target velocity
                velocity = rb.velocity;
                velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                rb.AddForce(velocityChange, ForceMode.VelocityChange);

                if (Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed || Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed)
                {
                    footStepTimer += Time.deltaTime;

                    if (footStepTimer >= walkFootStepSeconds && audioSource != null)
                    {
                        audioSource.pitch = Random.Range(0.9f, 1.1f);
                        audioSource.PlayOneShot(footStepSound);
                        footStepTimer = 0;
                    }
                }
            }

            // Jump
            if (canJump && Keyboard.current.spaceKey.isPressed)
            {
                rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalwalkSpeed(), velocity.z);
            }
        }
#endif

#if (ENABLE_LEGACY_INPUT_MANAGER)
        void LegacyInputSystem ()
        {
            // Calculate how fast we should be moving while running
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= runSpeed;

                // Apply a force that attempts to reach our target velocity
                velocity = rb.velocity;
                velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                rb.AddForce(velocityChange, ForceMode.VelocityChange);

                if (Input.GetKey(KeyCode.W))
                {
                    footStepTimer += Time.deltaTime;
                }

                if (footStepTimer >= runFootStepSeconds && audioSource != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(footStepSound);
                    footStepTimer = 0;
                }
            }

            // Calculate how fast we should be moving while walking
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= walkSpeed;

                // Apply a force that attempts to reach our target velocity
                velocity = rb.velocity;
                velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                rb.AddForce(velocityChange, ForceMode.VelocityChange);

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    footStepTimer += Time.deltaTime;

                    if (footStepTimer >= walkFootStepSeconds && audioSource != null)
                    {
                        audioSource.pitch = Random.Range(0.9f, 1.1f);
                        audioSource.PlayOneShot(footStepSound);
                        footStepTimer = 0;
                    }
                }
            }

            // Jump
            if (canJump && Input.GetButton("Jump"))
            {
                rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalwalkSpeed(), velocity.z);
            }
        }
#endif

        void OnCollisionStay(Collision col)
        {
            if (col.gameObject.tag == "Untagged" || !onlyJumpOnUntagged)
            {
                grounded = true;
            }
        }

        float CalculateJumpVerticalwalkSpeed()
        {
            // From the jump height and gravity we deduce the upwards walkSpeed 
            // for the character to reach at the apex.
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }
    }
}
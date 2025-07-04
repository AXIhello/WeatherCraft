using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    public CharacterController playerController;
    public float speed = 3;
    public float jumpPower = 3;
    public float gravity = 7f;
    public float mouseSensitivity = 1f;
    public InputActionReference moveActionRef;
    public Camera playerCamera;

    private Vector3 direction;
    private float yaw = 0f;
    private float pitch = 0f;
    private float smoothYaw = 0f;
    private float smoothPitch = 0f;
    public float smoothTime = 0.05f;
    private float yawVelocity;
    private float pitchVelocity;

    void Start()
    {
        moveActionRef.action.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 处理移动输入
        var keyboard = Keyboard.current;
        Vector2 moveInput = moveActionRef.action.ReadValue<Vector2>();
        float horizontal = moveInput.x;
        float vertical = moveInput.y;
        if (playerController.isGrounded)
        {
            direction = new Vector3(horizontal, 0, vertical);
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
            {
                direction.y = jumpPower;
            }
        }
        direction.y -= gravity * Time.deltaTime;
        
        // 如果按住Alt显示鼠标位置
        if (keyboard != null && keyboard.leftAltKey.isPressed) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        Cursor.visible = false;
        
        // 读取鼠标输入用于视角控制
        var mouse = Mouse.current;
        if (mouse != null)
        {
            float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity;
            float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity;
            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
        }
        // 只使用 Y 轴旋转转换移动方向
        Quaternion yawRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        playerController.Move(yawRotation * direction * (speed * Time.deltaTime));
    }

    void LateUpdate()
    {
        // 平滑旋转
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawVelocity, smoothTime);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, smoothYaw, 0);
        if (playerCamera)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(smoothPitch, 0, 0);
        }
    }
}
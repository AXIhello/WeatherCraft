using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    public CharacterController playerController;
    public float speed = 3;
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
        var keyboard = Keyboard.current;
        Vector2 moveInput = moveActionRef.action.ReadValue<Vector2>();
        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        // 始终获取水平移动，并根据 Q/E 键设置垂直移动
        direction = new Vector3(horizontal, 0, vertical);
        if (keyboard != null)
        {
            if (keyboard.eKey.isPressed)
            {
                direction.y = speed;
            }
            else if (keyboard.qKey.isPressed)
            {
                direction.y = -speed;
            }
            else
            {
                direction.y = 0; // 当没有上下方向输入时，不做垂直移动
            }
        }

        // 限制方向向量的长度，确保移动速度不会超过步行速度
        direction = Vector3.ClampMagnitude(direction, 1f);

        // 按住 Alt 时显示鼠标位置
        if (keyboard != null && keyboard.leftAltKey.isPressed)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        Cursor.visible = false;

        // 鼠标视角控制
        var mouse = Mouse.current;
        if (mouse != null)
        {
            float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity;
            float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity;
            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
        }

        // 根据当前 Y 轴旋转转换移动方向
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
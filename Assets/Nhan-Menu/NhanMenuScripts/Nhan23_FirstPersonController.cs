using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Nhan23_FirstPersonController : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Cài đặt chuột")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;
    [Range(30, 89)]
    public float lookLimit = 70f;   // ✅ Giới hạn góc nhìn dọc, giảm xuống để tránh nhìn xuyên đất

    private float cameraPitch = 0f; // Góc dọc hiện tại
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Giới hạn vị trí camera")]
    public float minCameraHeight = 1.2f; // ✅ Camera không thấp hơn mức này so với mặt đất

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Ẩn & khóa chuột
        Cursor.visible = false;
    }

    void Update()
    {
        LookAround();
        MovePlayer();
        KeepCameraAboveGround();
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Xoay thân nhân vật theo trục Y (trái/phải)
        transform.Rotate(Vector3.up * mouseX);

        // Xoay camera theo trục X (trên/dưới) có giới hạn
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -lookLimit, lookLimit); // ✅ Giới hạn góc nhìn
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void MovePlayer()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // Giữ dính mặt đất

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Áp dụng trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void KeepCameraAboveGround()
    {
        // ✅ Giữ camera không vượt qua mặt đất
        Ray ray = new Ray(playerCamera.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            float groundY = hit.point.y;
            float desiredY = groundY + minCameraHeight;

            if (playerCamera.position.y < desiredY)
            {
                Vector3 camPos = playerCamera.position;
                camPos.y = Mathf.Lerp(camPos.y, desiredY, Time.deltaTime * 10f);
                playerCamera.position = camPos;
            }
        }
    }
}

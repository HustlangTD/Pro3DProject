using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform cameraTransform;   // gắn MainCamera (có Cinemachine Brain)

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Kiểm tra nhân vật có đứng trên đất không
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Lấy input WASD
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Tính hướng theo góc nhìn camera
        if (direction.magnitude >= 0.1f)
        {
            // Lấy góc quay của camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Quay nhân vật theo hướng di chuyển
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            // Tính vector di chuyển theo hướng camera
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Check chạy hay đi bộ
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float speed = isRunning ? runSpeed : walkSpeed;

            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // Gửi dữ liệu vào Animator
            animator.SetFloat("walk", !isRunning ? direction.magnitude : 0f);
            animator.SetFloat("run", isRunning ? direction.magnitude : 0f);
        }
        else
        {
            animator.SetFloat("walk", 0f);
            animator.SetFloat("run", 0f);
        }

        // Nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetFloat("jump", 1f); // Kích hoạt animation jump
        }
        else
        {
            animator.SetFloat("jump", 0f);
        }

        // Áp dụng trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

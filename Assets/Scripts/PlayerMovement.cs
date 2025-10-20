using UnityEngine;
using Unity.Cinemachine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Transform cameraTransform; // Gắn CameraFollowPoint (con của player)
    public CinemachineCamera virtualCamera;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;

    public bool cutscenePlaying = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Nếu cameraTransform chưa gán, lấy main camera
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        
        Debug.Log($"Horizontal: {Input.GetAxis("Horizontal")} | Vertical: {Input.GetAxis("Vertical")} | Jump: {Input.GetButtonDown("Jump")}");
        if (cutscenePlaying) return;

        // Kiểm tra có đứng trên đất không
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Input WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool hasInput = direction.magnitude >= 0.1f;
        bool isRunning = hasInput && Input.GetKey(KeyCode.LeftShift);
        


        // Di chuyển theo hướng camera
        if (hasInput)
        {
            // Hướng di chuyển theo camera (FPS view)
            Vector3 moveDir = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
            moveDir.y = 0f; // tránh di chuyển theo trục Y (ngẩng lên/xuống)

            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // Quay player theo hướng camera (chỉ trục Y)
            Vector3 lookDir = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
        }

        // Animator (nếu có)
        if (animator)
        {
            animator.SetBool("walk", hasInput && !isRunning);
            animator.SetBool("run", isRunning);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator) animator.SetBool("jump", true);
        }
        else if (isGrounded && animator)
        {
            animator.SetBool("jump", false);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

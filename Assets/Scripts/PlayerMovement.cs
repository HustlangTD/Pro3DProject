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
    public Transform cameraTransform;
    public CinemachineCamera virtualCamera;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    public bool isGrounded { get; private set; } // Public get để test có thể check

    public bool cutscenePlaying = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (cutscenePlaying) return;

        // 1. Thu thập Input từ bàn phím
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isJumpPressed = Input.GetButtonDown("Jump");
        bool isRunHeld = Input.GetKey(KeyCode.LeftShift);

        // 2. Gọi hàm xử lý (Hàm này sẽ được test gọi riêng mà không cần bấm phím)
        HandleMovement(h, v, isJumpPressed, isRunHeld);
    }

    // Tách logic ra đây để Test có thể gọi trực tiếp
    public void HandleMovement(float horizontal, float vertical, bool jumpPressed, bool runHeld)
    {
        // Kiểm tra ground
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        bool hasInput = direction.magnitude >= 0.1f;
        bool isRunning = hasInput && runHeld;

        // Di chuyển
        if (hasInput && cameraTransform != null)
        {
            Vector3 moveDir = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
            moveDir.y = 0f;

            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            Vector3 lookDir = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
            }
        }

        // Animator
        if (animator)
        {
            animator.SetBool("walk", hasInput && !isRunning);
            animator.SetBool("run", isRunning);
        }

        // Jump
        if (jumpPressed && isGrounded)
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
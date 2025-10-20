using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementRigidbody : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;

    [Header("Camera Settings")]
    public Transform cameraHolder; // Empty chứa camera, nằm trên đầu player
    public float mouseSensitivity = 200f;

    [Header("Ground Check")]
    public Transform groundCheck; // Empty dưới chân player
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    private float xRotation = 0f; // Góc xoay dọc camera

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // tránh bị ngã khi va chạm

        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleAnimation();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xoay dọc (camera nhìn lên/xuống)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Xoay ngang (xoay cả player theo hướng camera)
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + mouseX, 0f);
    }


    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Hướng di chuyển dựa theo hướng player (đã xoay cùng camera)
        Vector3 direction = transform.forward * vertical + transform.right * horizontal;
        direction.Normalize();

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetVelocity = direction * speed;

        // Giữ vận tốc Y hiện tại
        Vector3 velocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleAnimation()
    {
        if (!animator) return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool hasInput = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        animator.SetBool("walk", hasInput && !isRunning);
        animator.SetBool("run", isRunning && hasInput);
        animator.SetBool("jump", !isGrounded);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NhanPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    public float minLookX = -70f;
    public float maxLookX = 75f;
    public float cameraHeight = 1.7f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private float xRotation = 0f;
    private float moveInputX;
    private float moveInputZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearDamping = 2f;
        rb.angularDamping = 5f;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        cameraTransform.SetParent(transform);
        cameraTransform.localPosition = new Vector3(0f, cameraHeight, 0f);
        cameraTransform.localRotation = Quaternion.identity;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        moveInputX = Input.GetAxisRaw("Horizontal");
        moveInputZ = Input.GetAxisRaw("Vertical");
        RotateWithMouse();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDir = (transform.right * moveInputX + transform.forward * moveInputZ).normalized;
        Vector3 targetPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    void RotateWithMouse()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookX, maxLookX);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}

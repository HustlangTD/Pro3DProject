using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NhanPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 100f;
    public Transform cameraTransform; // Camera sẽ được gắn vào nhân vật

    private Rigidbody rb;
    private bool isGrounded = true;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Đảm bảo camera được gán
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Đặt camera làm con của nhân vật và định vị ở vị trí "đầu"
        cameraTransform.SetParent(transform);
        cameraTransform.localPosition = new Vector3(0f, 1f, 0f); // Đặt camera ở vị trí đầu (điều chỉnh theo mô hình nhân vật)
        cameraTransform.localRotation = Quaternion.identity;

        // Khóa và ẩn con trỏ chuột
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();
        Jump();
        RotateWithMouse();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Di chuyển theo hướng tương đối với nhân vật
        Vector3 moveDir = (transform.right * h + transform.forward * v).normalized;
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void RotateWithMouse()
    {
        // Lấy input chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xoay nhân vật theo trục Y (trái-phải)
        transform.Rotate(Vector3.up * mouseX);

        // Xoay camera theo trục X (lên-xuống)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Giới hạn góc nhìn lên/xuống
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra va chạm với mặt đất
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
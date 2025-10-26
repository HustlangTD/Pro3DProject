using UnityEngine;

public class NhanCameraFirstPerson : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;              // Player (thường là nhân vật)
    public Vector3 offset = new Vector3(0f, 1.7f, 0f); // vị trí camera (đầu người)

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public float maxLookUp = 75f;
    public float maxLookDown = -60f;

    private float xRotation = 0f;         // góc nhìn dọc
    private float yRotation = 0f;         // góc xoay ngang (theo player)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target == null)
        {
            Debug.LogWarning("NhanCameraFirstPerson: Chưa gán target (Player).");
            return;
        }

        // Đặt camera ở vị trí đầu người
        transform.position = target.position + offset;
        transform.rotation = target.rotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Nhận input chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xử lý góc nhìn
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxLookDown, maxLookUp);

        // Xoay camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Xoay Player theo hướng ngang (y)
        target.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // Giữ camera ở vị trí đầu người
        transform.position = target.position + offset;
    }
}

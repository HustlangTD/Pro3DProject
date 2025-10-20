using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform playerBody;      // Thân player (để xoay ngang)
    public float mouseSensitivity = 200f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Ẩn & khóa chuột giữa màn hình
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xoay dọc (nhìn lên/xuống)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Xoay ngang (thân player)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

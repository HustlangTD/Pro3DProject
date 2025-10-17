using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;       // Nhân vật
    public float mouseSensitivity = 200f;

    private float pitch = 0f; // góc xoay dọc

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    void Update()
    {
        // Lấy input chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xoay ngang (yaw) -> xoay cả player
        player.Rotate(Vector3.up * mouseX);

        // Xoay dọc (pitch) -> chỉ camera
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 20f); // giới hạn ngẩng/cúi
        transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
}
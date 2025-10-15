using UnityEngine;

public class NhanCameraFollow : MonoBehaviour
{
    public Transform target;        // Player
    public Vector3 offset = new Vector3(0, 3, -6);
    public float followSpeed = 5f;
    public float rotateSpeed = 2f;

    private float yaw;
    private float pitch;

    void LateUpdate()
    {
        if (target == null) return;

        // Xoay camera bằng chuột
        yaw += Input.GetAxis("Mouse X") * rotateSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotateSpeed;
        pitch = Mathf.Clamp(pitch, -20f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Theo dõi player
        Vector3 desiredPosition = target.position + rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}

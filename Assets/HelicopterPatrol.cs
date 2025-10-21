using UnityEngine;

public class HelicopterPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;          // Tốc độ bay
    public float flyDuration = 5f;         // Thời gian bay mỗi chiều
    public float rotateDuration = 2f;      // Thời gian xoay 180 độ

    private Vector3 startPosition;
    private Quaternion targetRotation;
    private bool flyingForward = true;
    private bool rotating = false;
    private float timer = 0f;
    private float rotateTimer = 0f;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Nếu đang xoay, thì không di chuyển
        if (rotating)
        {
            rotateTimer += Time.deltaTime;
            float t = rotateTimer / rotateDuration;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            if (t >= 1f)
            {
                rotating = false;
                rotateTimer = 0f;
            }

            return;
        }

        // Di chuyển
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Đếm thời gian bay
        timer += Time.deltaTime;

        // Khi hết thời gian bay —> chuẩn bị xoay
        if (timer >= flyDuration)
        {
            timer = 0f;
            flyingForward = !flyingForward;

            // Gọi hàm xoay 180 độ
            RotateHelicopter();
        }
    }

    private void RotateHelicopter()
    {
        rotating = true;
        // Xoay 180 độ quanh trục Y
        targetRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * moveSpeed * flyDuration);
    }
}

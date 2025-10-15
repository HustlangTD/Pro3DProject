using UnityEngine;

public class SimpleMove : MonoBehaviour
{
   public float speed = 5f;          // tốc độ di chuyển
    public float gravity = -9.81f;    // lực hấp dẫn
    public float jumpHeight = 2f;     // độ cao nhảy

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // kiểm tra nhân vật có đứng trên mặt đất không
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // giữ nhân vật dính mặt đất
        }

        // input di chuyển
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        // nhân vật di chuyển theo hướng nhìn
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        // nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // áp dụng gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("Hit " + collision.gameObject.name + "!");
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            print("Hit a wall");
            Destroy(gameObject);
        }
    }
}

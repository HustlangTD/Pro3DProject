using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("Missile Settings")]
    [SerializeField] private float speed = 25f;               // Tốc độ bay
    [SerializeField] private float rotateSpeed = 5f;          // Độ nhanh khi quay đầu
    [SerializeField] private float explosionRadius = 15f;     // Bán kính nổ
    [SerializeField] private float explosionForce = 1200f;    // Lực nổ
    [SerializeField] private float damage = 200f;             // Sát thương
    [SerializeField] private GameObject explosionEffect;      // Hiệu ứng nổ
    [SerializeField] private float lifetime = 10f;            // Tự hủy sau 10s nếu không trúng gì

    private Rigidbody rb;
    private Transform target;
    private bool exploded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Không rơi tự do
        target = FindClosestEnemy();
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (exploded) return;

        if (target == null)
        {
            target = FindClosestEnemy(); // Tìm lại nếu mục tiêu chết
            if (target == null)
            {
                rb.linearVelocity = transform.forward * speed; // Bay thẳng nếu không có quái
                return;
            }
        }

        // Hướng về mục tiêu
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotateSpeed * Time.fixedDeltaTime);

        // Di chuyển tới mục tiêu
        rb.linearVelocity = transform.forward * speed;
    }

    private Transform FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Enemy e in enemies)
        {
            if (e.isDead) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = e.transform;
            }
        }
        return closest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded) return;
        exploded = true;

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var col in colliders)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage((int)damage);
        }

        Destroy(gameObject);
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;
    private NavMeshAgent navAgent;
    public bool isDead;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damageAmount)
    {
        // Nếu đã chết rồi thì không nhận thêm sát thương
        if (isDead) return; 

        HP -= damageAmount;

        if (HP <= 0) // Không cần check !isDead nữa vì đã check ở trên
        {
            isDead = true;

            // 1. Dừng NavMeshAgent ngay lập tức để không bị trôi
            if (navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.isStopped = true; // Dừng di chuyển ngay
                navAgent.enabled = false;  // Tắt component
            }

            // 2. Chơi animation chết (KHÔNG TẮT ANIMATOR)
            int randomValue = Random.Range(0, 2);
            animator.SetTrigger(randomValue == 0 ? "DIE1" : "DIE2");

            // 3. Tắt Animator đi (đã bị xóa)
            // if (animator != null)
            //     animator.enabled = false; // <-- XÓA DÒNG NÀY

            // 4. Phát âm thanh chết
            if (SoundManager.Instance != null)
                SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieDeath);

            // 5. Cộng điểm
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(100);

            // 6. Cho enemy rớt xuống layer vô hại
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            
            // (Tùy chọn) Tắt các Collider để đạn bay xuyên qua
            foreach (Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }

            // 7. Xóa sau 4 giây (cho animation chạy)
            Destroy(gameObject, 4f);
        }
        else // Nếu chưa chết (isDead == false)
        {
            animator.SetTrigger("DAMAGE");

            // Hurt sound
            if (SoundManager.Instance != null)
                SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieHurt);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f); // Attacking range

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f); // Detection range

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f); // Stop chasing
    }
}

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
        HP -= damageAmount;

        if (HP <= 0 && !isDead)
        {
            isDead = true;

            // Random animation chết
            int randomValue = Random.Range(0, 2);
            animator.SetTrigger(randomValue == 0 ? "DIE1" : "DIE2");

            // Tắt Animator để ZombieChaseState ngừng gọi SetDestination
            if (animator != null)
                animator.enabled = false;

            // Tắt NavMeshAgent an toàn
            if (navAgent != null && navAgent.isOnNavMesh)
                navAgent.enabled = false;

            // Dead sound
            if (SoundManager.Instance != null)
                SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieDeath);

            // Cộng điểm
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(100);

            // Cho enemy rớt xuống layer vô hại
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            // Xóa sau 4 giây
            Destroy(gameObject, 4f);
        }
        else if (!isDead)
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

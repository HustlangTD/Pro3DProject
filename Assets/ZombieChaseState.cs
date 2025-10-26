using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : StateMachineBehaviour
{

    Transform player;
    NavMeshAgent agent;
    Enemy enemyScript; // <-- MỚI: Thêm biến để giữ script Enemy

    public float chaseSpeed = 6f;

    public float stopChasingDistance = 21;
    public float attackingDistance = 2.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
        enemyScript = animator.GetComponent<Enemy>(); // <-- MỚI: Lấy script Enemy lúc bắt đầu
        
        agent.speed = chaseSpeed;
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ===========================================
        // ===== KIỂM TRA ĐIỀU KIỆN CHẾT (QUAN TRỌNG) =====
        // ===========================================
        if (enemyScript != null && enemyScript.isDead) // <-- MỚI: Nếu enemy đã chết
        {
            return; // <-- MỚI: Dừng mọi hành động đuổi theo
        }
        // ===========================================

        if (SoundManager.Instance.zombieChannel.isPlaying == false)
        {
            SoundManager.Instance.zombieChannel.PlayOneShot(SoundManager.Instance.zombieChase);
        }

        // Chỉ chạy các lệnh này nếu enemy còn sống
        agent.SetDestination(player.position);
        animator.transform.LookAt(player);

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        if (distanceFromPlayer > stopChasingDistance)
        {
            animator.SetBool("isChasing", false);
        }
        
        if (distanceFromPlayer < attackingDistance)
        {
            animator.SetBool("isAttacking", true);
        }
    }

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Kiểm tra agent còn tồn tại không (phòng trường hợp enemy bị destroy)
        if (agent != null && agent.isOnNavMesh && agent.enabled)
        {
            agent.SetDestination(agent.transform.position);
        }
        
        SoundManager.Instance.zombieChannel.Stop();
    }
}
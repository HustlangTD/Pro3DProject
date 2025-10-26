using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossBeetleAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;                       // Gán player
    public GameObject healthBarUI;                 // Thanh máu trên đầu boss
    public GameObject victoryPanel;                // Panel chiến thắng
    public Animator anim;                          // Animator của boss
    public NavMeshAgent agent;                     // NavMeshAgent
    public Transform[] patrolPoints;               // 4 điểm tuần tra
    public Transform[] attackPoints;               // 2 điểm chạm để gây damage

    [Header("Stats")]
    public int maxHP = 500;
    public int currentHP;
    public int damage = 30;
    public float detectionRange = 20f;             // Khoảng cách phát hiện player
    public float attackRange = 3f;                 // Khoảng cách tấn công player
    public float patrolWaitTime = 3f;              // Thời gian dừng lại giữa các điểm
    public float idleTime = 2f;                    // Thời gian idle giữa patrol

    [Header("Internal States")]
    private int patrolIndex = 0;
    private bool isPatrolling = true;
    private bool isIdle = false;
    private bool isRaging = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool hasTriggeredRage = false;

    private Player playerScript;

    void Start()
    {
        currentHP = maxHP;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        playerScript = player.GetComponent<Player>();
        healthBarUI.SetActive(false);
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Nếu chưa rage mà player lại gần
        if (!hasTriggeredRage && distanceToPlayer <= detectionRange)
        {
            StartCoroutine(EnterRage());
            return;
        }

        // Nếu đang rage hoặc idle thì không patrol
        if (isRaging || isIdle) return;

        // Nếu đã rage rồi => chase và attack
        if (hasTriggeredRage)
        {
            ChaseAndAttack(distanceToPlayer);
            return;
        }

        // Ngược lại, tiếp tục patrol
        Patrol();
    }

    // ===============================
    // ====== PATROL SYSTEM ==========
    // ===============================
    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isIdle)
        {
            StartCoroutine(PatrolWait());
        }
        anim.SetBool("isWalking", true);
    }

    IEnumerator PatrolWait()
    {
        isIdle = true;
        anim.SetBool("isWalking", false);
        anim.SetTrigger("Idle");

        yield return new WaitForSeconds(patrolWaitTime);

        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        GoToNextPatrolPoint();
        isIdle = false;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[patrolIndex].position;
        anim.SetBool("isWalking", true);
    }

    // ===============================
    // ====== RAGE PHASE =============
    // ===============================
    IEnumerator EnterRage()
    {
        hasTriggeredRage = true;
        isRaging = true;
        agent.isStopped = true;

        anim.SetTrigger("Rage");
        healthBarUI.SetActive(true);

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 0.5f);

        isRaging = false;
        agent.isStopped = false;
    }

    // ===============================
    // ====== CHASE + ATTACK =========
    // ===============================
    void ChaseAndAttack(float distanceToPlayer)
    {
        if (distanceToPlayer > attackRange)
        {
            // Chase player
            agent.isStopped = false;
            agent.SetDestination(player.position);
            anim.SetBool("isRunning", true);
        }
        else
        {
            // Attack
            agent.isStopped = true;
            anim.SetBool("isRunning", false);

            if (!isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // Random chọn attack 1 hoặc 2
        int attackIndex = Random.Range(1, 3);
        string attackAnim = (attackIndex == 1) ? "Attack1" : "Attack2";

        anim.SetTrigger(attackAnim);

        yield return new WaitForSeconds(0.5f); // delay trước khi gây damage
        DamagePlayer();

        yield return new WaitForSeconds(1.2f); // tổng thời gian tấn công
        isAttacking = false;
    }

    void DamagePlayer()
    {
        foreach (Transform point in attackPoints)
        {
            Collider[] hit = Physics.OverlapSphere(point.position, 0.7f);
            foreach (Collider col in hit)
            {
                if (col.CompareTag("Player"))
                {
                    playerScript.TakeDamage(damage);
                }
            }
        }
    }

    // ===============================
    // ====== DAMAGE + DEATH =========
    // ===============================
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHP -= dmg;
        if (currentHP <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        agent.isStopped = true;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 1f);

        // Hiện victory UI
        victoryPanel.SetActive(true);
        Time.timeScale = 0f; // dừng game

        // Unlock chuột để bấm nút
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ===============================
    // ====== DEBUG DRAW =============
    // ===============================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (Transform p in attackPoints)
        {
            Gizmos.DrawWireSphere(p.position, 0.7f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

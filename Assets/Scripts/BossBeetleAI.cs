using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BossBeetleAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Slider healthBar;
    public GameObject victoryPanel;
    public Animator anim;
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    public Transform[] attackPoints;

    [Header("Stats")]
    public int maxHP = 500;
    public int currentHP;
    public int damage = 30;
    public float detectionRange = 20f;
    public float attackRange = 3f;
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 5.5f;
    public float patrolWaitTime = 3f;
    public float idleTime = 2f;

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
        healthBar.maxValue = maxHP;     // <-- MỚI: Cài đặt giá trị tối đa cho thanh máu
        healthBar.value = currentHP;    // <-- MỚI: Cài đặt giá trị hiện tại
        healthBar.gameObject.SetActive(false); // <-- MỚI: Ẩn thanh máu
        GoToNextPatrolPoint();

        // 🦗 Phát âm thanh patrol khi bắt đầu
        SoundManager.Instance.PlayBossPatrolSound();
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Rage khi player lại gần
        if (!hasTriggeredRage && distanceToPlayer <= detectionRange)
        {
            StartCoroutine(EnterRage());
            return;
        }

        if (isRaging || isIdle) return;

        if (hasTriggeredRage)
        {
            ChaseAndAttack(distanceToPlayer);
            return;
        }

        Patrol();
    }

    // ===============================
    // ====== PATROL SYSTEM ==========
    // ===============================
    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isIdle)
        {
            StartCoroutine(PatrolWait());
        }

        anim.SetBool("isWalking", true);

        // 🔊 Bảo đảm âm thanh patrol chỉ phát một lần khi di chuyển
        if (!SoundManager.Instance.bossChannel.isPlaying)
        {
            SoundManager.Instance.PlayBossPatrolSound();
        }
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
        healthBar.gameObject.SetActive(true);

        // 🔊 Phát âm thanh Rage
        SoundManager.Instance.PlayBossRageSound();

        float rageAnimTime = 2.5f;
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains("rage"))
            {
                rageAnimTime = clip.length;
                break;
            }
        }

        yield return new WaitForSeconds(rageAnimTime + 0.3f);

        isRaging = false;
        agent.isStopped = false;
    }

    // ===============================
    // ====== CHASE + ATTACK =========
    // ===============================
    void ChaseAndAttack(float distanceToPlayer)
    {
        // Boss CHẠY tới player
        if (distanceToPlayer > attackRange - 0.5f)
        {
            if (agent.isStopped)
                agent.isStopped = false;

            anim.SetBool("isRunning", true);
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);

            
            anim.ResetTrigger("Attack1");
            anim.ResetTrigger("Attack2");

            // 🔊 Nếu chưa phát tiếng chạy
            if (!SoundManager.Instance.bossChannel.isPlaying)
                SoundManager.Instance.PlayBossRunSound();
        }
        else
        {
            // Boss trong tầm tấn công
            agent.isStopped = true;
            anim.SetBool("isRunning", false);

            if (!isAttacking)
                StartCoroutine(AttackPlayer());
        }
    }


    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // 🔁 Boss chọn ngẫu nhiên 1 trong 2 chiêu mỗi lần
        int attackIndex = Random.Range(1, 3);
        string attackAnim = (attackIndex == 1) ? "Attack1" : "Attack2";

        // Reset 2 trigger để đảm bảo Animator không giữ trigger cũ
        anim.ResetTrigger("Attack1");
        anim.ResetTrigger("Attack2");

        // Set trigger tấn công ngẫu nhiên
        anim.SetTrigger(attackAnim);

        // 🔊 Phát âm thanh tấn công
        SoundManager.Instance.PlayBossAttackSound();

        // Đợi nửa giây để khớp với thời điểm ra đòn
        yield return new WaitForSeconds(0.5f);
        DamagePlayer();

        // 🕐 Lấy độ dài animation tương ứng để đợi xong trước khi tấn công tiếp
        float attackDuration = 1.2f;
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains(attackAnim.ToLower()))
            {
                attackDuration = clip.length;
                break;
            }
        }

        // Chờ hết animation rồi mới cho phép đánh tiếp
        yield return new WaitForSeconds(attackDuration);

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
        healthBar.value = currentHP;
        
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

        // 🔊 Âm thanh chết
        SoundManager.Instance.PlayBossDeathSound();

        float deathAnimTime = 3f;
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains("death"))
            {
                deathAnimTime = clip.length;
                break;
            }
        }

        yield return new WaitForSeconds(deathAnimTime + 1f);

        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
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

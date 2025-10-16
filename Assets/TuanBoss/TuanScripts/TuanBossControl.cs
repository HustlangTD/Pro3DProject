using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;



public class EnemyAI : MonoBehaviour
{

    public float health = 100f;
    public Transform playerTransform;
    public float sightRange = 15f;
    public float meleeRange = 2f;
    public float runAwayHealthThreshold = 25f;
    public bool isDead = false;
    private NavMeshAgent _agent;
    public float rotationSpeed = 5f;

    public float patrolSpeed = 2f;
    public float minDistanceToWaypoint = 1f;
    public List<Transform> patrolPoints;
    private int currentWaypointIndex = 0;
    public GameObject projectilePrefab;

    public Transform projectileSpawnPoint;
    public AudioClip meleeAttackSFX;
    private AudioSource audioSource;
    public ParticleSystem meleeAttackVFX;

    private Animator anim;
    private BehaviorTree behaviorTree;


    // Tên các tham số Animator
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";
    private const string TRIGGER_MELEE = "MeleeAttack";
    private const string TRIGGER_RANGED = "RangedAttack";
    private const string TRIGGER_DIE = "Die";


    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogError("ENEMY: Không tìm thấy AudioSource component!");
        }
        // Lấy Component Animator
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("ENEMY: Không tìm thấy Animator component!");
        }

        // Khởi tạo PlayerTransform 
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("ENEMY: Không tìm thấy NavMeshAgent component!");
        }

        if (_agent != null && !_agent.isOnNavMesh)
        {
            Debug.LogError("ENEMY: NavMeshAgent không được đặt trên NavMesh!");
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _agent.Warp(hit.position);
                Debug.Log("ENEMY: Đã di chuyển NavMeshAgent đến vị trí hợp lệ trên NavMesh.");
            }
            else
            {
                Debug.LogError("ENEMY: Không thể tìm thấy vị trí hợp lệ trên NavMesh gần vị trí hiện tại.");
            }
        }


        //Xây dựng Cây Hành vi
        Node root = SetupBehaviorTree();
        behaviorTree = new BehaviorTree(root);
    }


    void Update()
    {
        if (!isDead)
        {

            behaviorTree.Update();
        }
    }
    private void FacePlayer()
    {
        if(playerTransform == null) return;
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0; // Giữ nguyên trục Y để tránh nghiêng
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    private Node SetupBehaviorTree()
    {
        // 1. Nhánh Chết 
        ConditionNode isDeadCondition = new ConditionNode(IsDead);
        ActionNode DieAction = new ActionNode(Die);
        SequenceNode dieSequence = new SequenceNode(new List<Node> { isDeadCondition, DieAction });

        // 1b. Nhánh Chạy trốn
        ConditionNode isLowHealthCondition = new ConditionNode(IsHealthLow);
        ActionNode RunAwayAction = new ActionNode(RunAway);
        SequenceNode runAwaySequence = new SequenceNode(new List<Node> { isLowHealthCondition, RunAwayAction });

        // 1c. Nhánh Chiến đấu 
        ConditionNode isPlayerInSight = new ConditionNode(IsPlayerInSight);
        ConditionNode isPlayerClose = new ConditionNode(IsPlayerClose);
        ActionNode MeleeAttackAction = new ActionNode(MeleeAttack);
        SequenceNode meleeSequence = new SequenceNode(new List<Node> { isPlayerClose, MeleeAttackAction });

        ActionNode RangedAttackAction = new ActionNode(RangedAttack);

        SelectorNode attackSelector = new SelectorNode(new List<Node> { meleeSequence, RangedAttackAction });
        SequenceNode fightSequence = new SequenceNode(new List<Node> { isPlayerInSight, attackSelector });

        // 2. Nhánh Mặc định (Tuần tra)
        ActionNode PatrolAction = new ActionNode(Patrol);

        SelectorNode rootSelector = new SelectorNode(new List<Node>
        {
            dieSequence,
            runAwaySequence,
            fightSequence,
            PatrolAction
        });

        return rootSelector;
    }

    private bool IsDead()
    {
        return health <= 0;
    }
    private bool IsHealthLow()
    {
        return health < runAwayHealthThreshold;
    }
    private bool IsPlayerInSight()
    {
        if (playerTransform == null) return false;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= sightRange;
    }
    private bool IsPlayerClose()
    {
        if (playerTransform == null) return false;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= meleeRange;
    }

    private NodeState Die()
    {
        if (!isDead)
        {
            isDead = true;
            Debug.Log("ENEMY: 💀 CHẾT. Kích hoạt hoạt ảnh chết.");

            // Tắt tất cả các tham số di chuyển
            anim.SetBool(IS_WALKING, false);
            anim.SetBool(IS_RUNNING, false);
            anim.SetTrigger(TRIGGER_DIE);


        }
        return NodeState.SUCCESS;
    }

    private NodeState RunAway()
    {
        Debug.Log("ENEMY: 🏃 Máu thấp! Chạy trốn khỏi Player.");
        if (_agent == null) return NodeState.FAILURE;

        // Cài đặt hoạt ảnh chạy
        anim.SetBool(IS_WALKING, false);
        anim.SetBool(IS_RUNNING, true);

        // Thêm logic di chuyển để chạy xa player
        if (_agent.remainingDistance <= minDistanceToWaypoint || !_agent.hasPath)
        {
            Vector3 runDirection = (transform.position - playerTransform.position).normalized;
            Vector3 potentialDestination = transform.position + runDirection * 20f; // Chạy 20 đơn vị ra xa player
            if (NavMesh.SamplePosition(potentialDestination, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
                _agent.speed = patrolSpeed * 1.5f; // Chạy nhanh hơn tốc độ tuần tra
            }
            else
            {
                return NodeState.FAILURE; // Không tìm được điểm đến hợp lệ
            }
        }

        return NodeState.RUNNING;
    }

    private NodeState MeleeAttack()
    {
        Debug.Log("ENEMY: ⚔️ Player ở gần. Tấn công cận chiến!");
        FacePlayer();

        // Tắt di chuyển khi tấn công
        anim.SetBool(IS_WALKING, false);
        anim.SetBool(IS_RUNNING, false);

        // Kích hoạt hoạt ảnh tấn công cận chiến 
        anim.SetTrigger(TRIGGER_MELEE);
        return NodeState.SUCCESS;
    }

    private NodeState RangedAttack()
    {
        Debug.Log("ENEMY: 🔫 Player ở xa. Bắn!");
        _agent.isStopped = true; // Dừng di chuyển khi tấn công
        FacePlayer();


        anim.SetBool(IS_WALKING, false);
        anim.SetBool(IS_RUNNING, false);


        anim.SetTrigger(TRIGGER_RANGED);

        return NodeState.SUCCESS;
    }

    public void ShootProjectile()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError("ENEMY: projectilePrefab hoặc projectileSpawnPoint chưa được gán!");
            return;
        }
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        float projectileSpeed = 20f; // Tốc độ viên đạn
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = projectileSpawnPoint.forward * projectileSpeed;
        }
        else
        {
            ProjectileMover mover = projectile.GetComponent<ProjectileMover>();
            if (mover != null)
            {
                mover.SetDirection(projectileSpawnPoint.forward);
            }
            else
            {
                Debug.LogError("ENEMY: Không tìm thấy Rigidbody hoặc ProjectileMover trên projectilePrefab!");
            }
        }

    }

   // public void PlayMeleeSFX()
   // {
     //   if (meleeAttackSFX != null && audioSource != null)
    //    {
    //        audioSource.PlayOneShot(meleeAttackSFX);
      //  }
      //  else
      //  {
           // Debug.LogError("ENEMY: meleeAttackSFX hoặc audioSource chưa được gán!");
       // }
   // }

    public void PlayMeleeVFX()
    {
        if (meleeAttackVFX != null)
        {
            meleeAttackVFX.Play();
        }
        else
        {
            Debug.LogError("ENEMY: meleeAttackVFX chưa được gán!");
        }
    }
    private NodeState Patrol()
    {
        Debug.Log("ENEMY: 🚶 Tuần tra trong khu vực.");
        _agent.speed = patrolSpeed;

        anim.SetBool(IS_RUNNING, false);
        anim.SetBool(IS_WALKING, true);

        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            _agent.isStopped = true;
            return NodeState.FAILURE; // Không có điểm tuần tra
        }
        _agent.isStopped = false;
        _agent.SetDestination(patrolPoints[currentWaypointIndex].position);
        if (_agent.remainingDistance <= minDistanceToWaypoint && !_agent.pathPending)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Count;
        }

        return NodeState.RUNNING;
    }
}


internal class ProjectileMover
{
    internal void SetDirection(Vector3 direction)
    {
        throw new NotImplementedException();
    }

    internal void SetDirection(object direction)
    {
        throw new NotImplementedException();
    }
}

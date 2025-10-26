using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolingState : StateMachineBehaviour
{
    float timer;
    public float patrolingTime = 10f;

    Transform player;
    NavMeshAgent agent;
    Enemy enemyScript; // <-- MỚI: Thêm biến tham chiếu đến script Enemy

    public float detectionArea = 18f;
    public float patrolSpeed = 2f;

    List<Transform> waypointsList = new List<Transform>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
        enemyScript = animator.GetComponent<Enemy>(); // <-- MỚI: Lấy script Enemy

        timer = 0;

        // Lấy danh sách waypoints
        GameObject waypointCluster = GameObject.FindGameObjectWithTag("Waypoints");
        waypointsList.Clear(); // <-- MỚI: Xóa list cũ để tránh bị trùng lặp
        foreach (Transform t in waypointCluster.transform)
        {
            waypointsList.Add(t);
        }

        // KIỂM TRA AN TOÀN: Chỉ thực thi nếu agent còn hoạt động
        if (agent.enabled && agent.isOnNavMesh) // <-- MỚI
        {
            agent.speed = patrolSpeed;
            Vector3 nextPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
            agent.SetDestination(nextPosition);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // KIỂM TRA QUAN TRỌNG: Nếu enemy đã chết, không làm gì cả
        if (enemyScript != null && enemyScript.isDead) // <-- MỚI
        {
            return; // <-- MỚI
        }

        // Phát âm thanh
        if (SoundManager.Instance.zombieChannel.isPlaying == false)
        {
            SoundManager.Instance.zombieChannel.clip = SoundManager.Instance.zombieWalking;
            SoundManager.Instance.zombieChannel.PlayDelayed(1f);
        }

        // KIỂM TRA AN TOÀN: Chỉ kiểm tra remainingDistance khi agent còn sống
        if (agent.enabled && agent.isOnNavMesh) // <-- MỚI
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
            }
        }

        // Timer đếm ngược
        timer += Time.deltaTime;
        if (timer > patrolingTime)
        {
            animator.SetBool("isPatroling", false);
        }   

        // Kiểm tra player
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer < detectionArea)
        {
            animator.SetBool("isChasing", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // KIỂM TRA AN TOÀN: Chỉ SetDestination nếu agent còn hoạt động
        if (agent != null && agent.enabled && agent.isOnNavMesh) // <-- MỚI
        {
            agent.SetDestination(agent.transform.position);
        }
        
        SoundManager.Instance.zombieChannel.Stop();
    }
}
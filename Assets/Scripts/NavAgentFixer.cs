using UnityEngine;
using UnityEngine.AI;

public class NavAgentFixer : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent == null) return;

        // Nếu agent bị lọt khỏi NavMesh hoặc chưa được đặt lên
        if (!agent.isOnNavMesh)
        {
            // Dừng mọi hoạt động di chuyển
            agent.enabled = false;
        }
    }
}

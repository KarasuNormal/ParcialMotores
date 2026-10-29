
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform player;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        agent.SetDestination(player.position);
    }
}

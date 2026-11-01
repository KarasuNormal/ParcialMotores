using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private int damage = 1;

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

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();

        if (health != null)
            health.TakeDamage(damage);
    }
}
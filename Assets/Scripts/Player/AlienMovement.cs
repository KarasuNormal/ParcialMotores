
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 8f;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [Tooltip("Distancia horizontal al jugador para poder pegarle (metros)")]
    [SerializeField] private float attackRange = 1.5f;
    [Tooltip("Diferencia de altura maxima para pegar (evita pegar a traves de pisos)")]
    [SerializeField] private float attackHeightTolerance = 2.5f;
    [Tooltip("Segundos entre golpe y golpe")]
    [SerializeField] private float attackCooldown = 1f;

    [Header("NavMesh")]
    [Tooltip("Si el alien arranca fuera del NavMesh, busca el punto valido mas cercano dentro de esta distancia")]
    [SerializeField] private float navMeshSnapDistance = 5f;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null) playerHealth = player.GetComponentInChildren<PlayerHealth>();
        }

        // Si el alien quedo apenas fuera del NavMesh (encima o al costado del techo),
        // lo acomodamos sobre el punto valido mas cercano para que pueda moverse.
        if (agent != null && !agent.isOnNavMesh &&
            NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSnapDistance, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning(name + " no esta sobre un NavMesh: no va a poder perseguir al jugador.", this);
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (agent != null && agent.isOnNavMesh)
        {
            if (distance <= detectionRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = true;
            }
        }

        TryAttack();
    }

    // El golpe se decide por distancia: no depende de que los colliders sean trigger
    private void TryAttack()
    {
        if (playerHealth == null || playerHealth.IsDead || Time.time < nextAttackTime)
            return;

        Vector3 offset = player.position - transform.position;
        float heightDifference = Mathf.Abs(offset.y);
        offset.y = 0f;

        if (offset.magnitude > attackRange || heightDifference > attackHeightTolerance)
            return;

        nextAttackTime = Time.time + attackCooldown;
        playerHealth.TakeDamage(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolDelay = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float stopDistance = 2f;

    [Header("Idle Timing")]
    [SerializeField] private float idlePauseTime = 1.5f;

    [Header("Attack Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDelay = 0.5f; // décalage avant le hit

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 initialPosition;

    private float patrolCooldown;
    private bool isWaiting = false;

    private bool isAttacking = false;
    private float attackTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;

        agent.autoBraking = false;
        agent.acceleration = 100f;
        agent.angularSpeed = 999f;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                isAttacking = false;
                agent.isStopped = false;
                ChasePlayer();
            }
            else if (attackTimer <= 0f && !isAttacking)
            {
                StartAttack();
            }
        }
        else
        {
            Patrol();
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // rotation vers le joueur
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero) transform.forward = dir;

        animator.SetTrigger("Attack");

        // décalage des dégâts pour que ça colle à l'animation
        Invoke(nameof(DealDamage), attackDelay);

        // cooldown général
        attackTimer = attackCooldown;
    }

    void DealDamage()
    {
        // sécurité : vérifier que le joueur est toujours en portée
        if (Vector3.Distance(transform.position, player.position) <= stopDistance + 0.5f)
        {
            Health targetHealth = player.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }

        isAttacking = false;
        agent.isStopped = false;
    }

    private void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void Patrol()
    {
        if (isWaiting) return;

        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolCooldown -= Time.deltaTime;
            if (patrolCooldown <= 0f)
            {
                StartCoroutine(PauseBeforeNextPatrol());
            }
        }
        else
        {
            agent.velocity = agent.desiredVelocity.normalized * patrolSpeed;
        }
    }

    private IEnumerator PauseBeforeNextPatrol()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(idlePauseTime);

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += initialPosition;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        agent.isStopped = false;
        patrolCooldown = patrolDelay;
        isWaiting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}

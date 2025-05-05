using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MageEnemy : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private float retreatDistance = 3f;
    [SerializeField] private float retreatLength = 5f;
    [SerializeField] private float retreatCooldown = 3f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 10f;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolDelay = 2f;

    [Header("Target")]
    [SerializeField] private Transform player;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private float lastRetreatTime;
    private Vector3 initialPosition;
    private Vector3 patrolTarget;
    private float patrolCooldown;
    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        patrolCooldown = patrolDelay;

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (distance <= attackRange && distance > retreatDistance)
            {
                agent.SetDestination(transform.position); // Stop moving
                FacePlayer();

                if (Time.time - lastAttackTime >= attackCooldown)
                    Attack();
            }
            else if (distance <= retreatDistance && Time.time - lastRetreatTime >= retreatCooldown)
            {
                RetreatFromPlayer();
            }
            else
            {
                agent.SetDestination(player.position); // Follow the player
            }
        }
        else
        {
            Patrol();
        }
    }

    private void RetreatFromPlayer()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        Vector3 retreatPos = transform.position + dir * retreatLength;

        if (NavMesh.SamplePosition(retreatPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(retreatPos); // fallback if sample fails
        }

        lastRetreatTime = Time.time;
    }

    private void Attack()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 dir = (player.position + Vector3.up) - firePoint.position;
        projectile.GetComponent<Rigidbody>().velocity = dir.normalized * projectileSpeed;

        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Attack");
    }

    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolCooldown -= Time.deltaTime;
            if (patrolCooldown <= 0f)
            {
                Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
                randomDir += initialPosition;

                if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
                {
                    patrolTarget = hit.position;
                    agent.SetDestination(patrolTarget);
                }

                patrolCooldown = patrolDelay;
            }
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}

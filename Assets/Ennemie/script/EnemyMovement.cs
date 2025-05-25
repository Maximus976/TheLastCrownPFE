using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolDelay = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 8f;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float stopDistance = 2f;

    [Header("Idle Timing")]
    [SerializeField] private float idlePauseTime = 1.5f;

    [Header("Attack Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDelay = 0.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 initialPosition;
    private Transform playerTarget;

    private float patrolCooldown;
    private bool isWaiting = false;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private bool isDead = false;
    private bool isStunned = false;
    private bool canMove = true;

    private Coroutine stunCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;

        agent.autoBraking = false;
        agent.acceleration = 100f;
        agent.angularSpeed = 999f;
        agent.updatePosition = false;
        agent.updateRotation = false;

        animator.SetInteger("HitType", 0);
    }

    void Update()
    {
        if (isDead || isStunned || !canMove) return;

        playerTarget = FindClosestPlayer();
        if (playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                // Toujours en chasse
                isAttacking = false;
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(playerTarget.position);
                }
            }
            else
            {
                // Trop proche -> arrêt
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }

                // Peut attaquer si cooldown terminé
                if (attackTimer <= 0f && !isAttacking)
                {
                    StartAttack();
                }
            }
        }


        if (agent.isOnNavMesh && canMove)
        {
            transform.position = agent.nextPosition;

            Vector3 velocity = agent.desiredVelocity;
            if (velocity != Vector3.zero)
            {
                Quaternion toRot = Quaternion.LookRotation(velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * 10f);
            }
        }

        animator.SetFloat("Speed", isDead ? 0f : agent.velocity.magnitude, 0.1f, Time.deltaTime);
    }

    Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = player.transform;
            }
        }

        return closest;
    }

    void StartAttack()
    {
        isAttacking = true;
        if (agent.isOnNavMesh) agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 dir = (playerTarget.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion toRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * 15f);
        }

        animator.SetTrigger("Attack");

        Invoke(nameof(ApplyDamageToPlayer), attackDelay);

        attackTimer = attackCooldown;
    }

    void ApplyDamageToPlayer()
    {
        if (playerTarget == null) return;

        float distance = Vector3.Distance(transform.position, playerTarget.position);
        if (distance > stopDistance + 0.5f) return;

        Health health = playerTarget.GetComponent<Health>();
        if (health == null)
            health = playerTarget.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"[Enemy] Dégâts infligés à {playerTarget.name} : {damage}");
        }

        isAttacking = false;
        if (agent.isOnNavMesh) agent.isStopped = false;
    }

    private void Patrol()
    {
        if (isWaiting || !agent.isOnNavMesh) return;

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
        if (agent.isOnNavMesh) agent.isStopped = true;

        yield return new WaitForSeconds(idlePauseTime);

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += initialPosition;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            if (agent.isOnNavMesh)
                agent.SetDestination(hit.position);
        }

        if (agent.isOnNavMesh) agent.isStopped = false;
        patrolCooldown = patrolDelay;
        isWaiting = false;
    }

    public void ReactToHit(Vector3 attackerPosition)
    {
        if (isDead || isStunned) return;

        isStunned = true;

        Vector3 dir = (transform.position - attackerPosition).normalized;
        dir.y = 0;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(HitReaction(dir));
    }

    private IEnumerator HitReaction(Vector3 direction)
    {
        float knockbackDuration = 0.15f;
        float timer = 0f;

        if (agent.isOnNavMesh)
            agent.isStopped = true;

        while (timer < knockbackDuration)
        {
            transform.position += direction * 2f * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        if (agent.isOnNavMesh)
            agent.isStopped = false;

        isStunned = false;
        stunCoroutine = null;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        canMove = false;

        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        agent.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        int randomDeath = Random.Range(0, 4);
        animator.SetInteger("DeathIndex", randomDeath);
        animator.SetTrigger("Die");
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

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

    [Header("Hit Reaction")]
    [SerializeField] private float hitStunDuration = 0.2f;
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float knockbackTime = 0.15f;

    [Header("Feedback")]
    [SerializeField] private Renderer[] renderersToFlash;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

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

        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTarget == null)
        {
            Debug.LogError("Aucun objet avec le tag 'Player' trouvé pour EnemyMovement.");
            enabled = false;
            return;
        }

        agent.autoBraking = false;
        agent.acceleration = 100f;
        agent.angularSpeed = 999f;
        agent.updatePosition = false;
        agent.updateRotation = false;

        animator.SetInteger("HitType", 0);
    }

    void Update()
    {
        if (isDead || isStunned || playerTarget == null || !canMove) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                isAttacking = false;
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(playerTarget.position);
                }
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

        animator.SetFloat("Speed", isDead ? 0f : agent.velocity.magnitude);
    }

    void StartAttack()
    {
        isAttacking = true;
        if (agent.isOnNavMesh) agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 dir = (playerTarget.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero) transform.forward = dir;

        animator.SetTrigger("Attack");
        Invoke(nameof(DealDamage), attackDelay);
        attackTimer = attackCooldown;
    }

    void DealDamage()
    {
        if (Vector3.Distance(transform.position, playerTarget.position) <= stopDistance + 0.5f)
        {
            Health targetHealth = playerTarget.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
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
        if (isDead) return;

        isStunned = true;
        FlashRed();

        int hitType = Random.Range(1, 3);
        animator.SetInteger("HitType", hitType);
        animator.Update(0);
        animator.SetInteger("HitType", 0);

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(HitReactionRoutine(hitType, attackerPosition));
    }

    private IEnumerator HitReactionRoutine(int hitType, Vector3 attackerPosition)
    {
        if (agent.isOnNavMesh) agent.isStopped = true;

        if (hitType == 2)
        {
            Vector3 dir = (transform.position - attackerPosition).normalized;
            dir.y = 0;
            float timer = 0f;
            while (timer < knockbackTime)
            {
                agent.Move(dir * knockbackForce * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(knockbackTime);
        }

        yield return new WaitForSeconds(hitStunDuration);

        if (agent.isOnNavMesh) agent.isStopped = false;
        isStunned = false;
        stunCoroutine = null;
    }

    private void FlashRed()
    {
        foreach (Renderer rend in renderersToFlash)
        {
            StartCoroutine(FlashRoutine(rend));
        }
    }

    private IEnumerator FlashRoutine(Renderer rend)
    {
        if (rend == null) yield break;

        Material mat = rend.material;

        bool hasBaseColor = mat.HasProperty("_BaseColor");
        bool hasEmission = mat.HasProperty("_EmissionColor");

        Color originalColor = hasBaseColor ? mat.GetColor("_BaseColor") : Color.white;
        Color originalEmission = hasEmission ? mat.GetColor("_EmissionColor") : Color.black;

        if (hasBaseColor)
            mat.SetColor("_BaseColor", flashColor);

        if (hasEmission)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", flashColor * 5f);
        }

        yield return new WaitForSeconds(flashDuration);

        if (hasBaseColor)
            mat.SetColor("_BaseColor", originalColor);

        if (hasEmission)
        {
            mat.SetColor("_EmissionColor", originalEmission);
            if (originalEmission == Color.black)
                mat.DisableKeyword("_EMISSION");
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        canMove = false;

        // Arrête le NavMeshAgent
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        // ❌ Désactive le NavMeshAgent complètement
        agent.enabled = false;

        // Bloque les mouvements physiques
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Désactive le collider
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        // Joue une animation de mort
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

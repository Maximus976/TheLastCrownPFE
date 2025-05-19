using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MageEnemy : MonoBehaviour
{
    private enum MageState
    {
        Idle,
        Chasing,
        PreparingAttack,
        Attacking,
        PreparingRetreat,
        Retreating,
        Paralyzed
    }

    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float tooCloseDistance = 4f;
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float retreatCooldown = 5f;
    [SerializeField] private float retreatDistance = 5f;
    [SerializeField] private float retreatDuration = 2f;
    [SerializeField] private float paralyzeDuration = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 12f;

    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Visual Feedback")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Hit Reaction")]
    [SerializeField] private float hitStunDuration = 0.2f;
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float knockbackTime = 0.15f;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyHealth health;
    private MageState currentState = MageState.Idle;
    private float stateTimer;
    private float lastAttackTime;
    private float lastRetreatTime;
    private bool wasRecentlyHit = false;
    private bool playedChargeAnim = false;
    private bool isStunned = false;
    private Coroutine stunCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || isStunned || (health != null && health.IsDead)) return;

        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude, 0.1f, Time.deltaTime);
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (currentState == MageState.PreparingAttack && distance > attackRange)
        {
            currentState = MageState.Chasing;
            playedChargeAnim = false;
            return;
        }

        switch (currentState)
        {
            case MageState.Idle:
                HandleIdle(distance);
                break;
            case MageState.Chasing:
                HandleChasing(distance);
                break;
            case MageState.PreparingAttack:
                HandlePreparingAttack(distance);
                break;
            case MageState.Attacking:
                HandleAttacking(distance);
                break;
            case MageState.PreparingRetreat:
                HandlePreparingRetreat(distance);
                break;
            case MageState.Retreating:
                HandleRetreating();
                break;
            case MageState.Paralyzed:
                HandleParalyzed();
                break;
        }
    }

    private void HandleIdle(float distance)
    {
        if (distance <= detectionRange)
        {
            currentState = MageState.Chasing;
        }
    }

    private void HandleChasing(float distance)
    {
        if (distance <= attackRange)
        {
            agent.SetDestination(transform.position);
            stateTimer = attackDelay;
            playedChargeAnim = false;
            currentState = MageState.PreparingAttack;
        }
        else
        {
            agent.SetDestination(player.position);
            FacePlayer();
        }
    }

    private void HandlePreparingAttack(float distance)
    {
        FacePlayer();
        ForceStopMovement();

        if (!playedChargeAnim && animator != null)
        {
            animator.SetInteger("HitTypeMage", 1);
            animator.Update(0);
            animator.SetInteger("HitTypeMage", 0);
            playedChargeAnim = true;
        }

        stateTimer -= Time.deltaTime;

        if (wasRecentlyHit) return;

        if (stateTimer <= 0f)
        {
            Attack();
            currentState = MageState.Attacking;
        }
    }

    private void HandleAttacking(float distance)
    {
        FacePlayer();
        ForceStopMovement();

        if (distance <= tooCloseDistance && Time.time - lastRetreatTime >= retreatCooldown)
        {
            stateTimer = 0.5f;
            currentState = MageState.PreparingRetreat;
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (distance <= attackRange)
            {
                stateTimer = attackDelay;
                playedChargeAnim = false;
                currentState = MageState.PreparingAttack;
            }
            else
            {
                currentState = MageState.Chasing;
            }
        }

        if (wasRecentlyHit && distance <= tooCloseDistance)
        {
            stateTimer = paralyzeDuration;
            currentState = MageState.Paralyzed;
        }
    }

    private void HandlePreparingRetreat(float distance)
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            Vector3 retreatDir = (transform.position - player.position).normalized;
            Vector3 targetPos = transform.position + retreatDir * retreatDistance;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                stateTimer = retreatDuration;
                lastRetreatTime = Time.time;
                currentState = MageState.Retreating;
            }
            else
            {
                currentState = MageState.Chasing;
            }
        }
    }

    private void HandleRetreating()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            agent.SetDestination(transform.position);
            currentState = MageState.Attacking;
        }
    }

    private void HandleParalyzed()
    {
        ForceStopMovement();

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            wasRecentlyHit = false;
            currentState = MageState.Attacking;
        }
    }

    private void Attack()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector3 dir = (player.position + Vector3.up) - firePoint.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Quaternion correction = Quaternion.Euler(0, -90, 0);
        Quaternion finalRotation = lookRotation * correction;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, finalRotation);
        projectile.GetComponent<Rigidbody>().velocity = dir.normalized * projectileSpeed;

        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetInteger("HitTypeMage", 2);
            animator.Update(0);
            animator.SetInteger("HitTypeMage", 0);
        }
    }

    public void ReceiveHit()
    {
        if (isStunned || (health != null && health.IsDead)) return;

        wasRecentlyHit = true;
        currentState = MageState.Attacking;
        lastAttackTime = Time.time; // impose le cooldown

        ForceStopMovement();

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetInteger("HitTypeMage", 0);
        }

        int hitType = Random.Range(1, 3);
        if (animator != null)
        {
            animator.SetInteger("HitType", hitType);
            animator.Update(0);
            animator.SetInteger("HitType", 0);
        }

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(HitReactionRoutine(hitType));

        if (meshRenderer != null)
            StartCoroutine(FlashColor());
    }

    private IEnumerator HitReactionRoutine(int hitType)
    {
        isStunned = true;
        if (agent.isOnNavMesh) agent.isStopped = true;

        ForceStopMovement();

        if (hitType == 2)
        {
            Vector3 dir = (transform.position - player.position).normalized;
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
        wasRecentlyHit = false;
        stunCoroutine = null;
    }

    private IEnumerator FlashColor()
    {
        if (meshRenderer == null) yield break;

        Material mat = meshRenderer.material;

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

    private void CancelCurrentBehavior()
    {
        agent.ResetPath();
        stateTimer = 0f;
        playedChargeAnim = false;
        currentState = MageState.Idle;

        if (animator != null)
        {
            animator.SetInteger("HitTypeMage", 0);
            animator.SetFloat("Speed", 0f);
        }

        ForceStopMovement();
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

    private void ForceStopMovement()
    {
        if (agent == null) return;

        agent.SetDestination(transform.position);
        agent.velocity = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, tooCloseDistance);
    }
}

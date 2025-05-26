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

    [Header("Visual Feedback")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Hit Reaction")]
    [SerializeField] private float stunDuration = 1f;
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
    private Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        FindTarget();
    }

    void Update()
    {
        if (target == null || isStunned || (health != null && health.IsDead)) return;

        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude, 0.1f, Time.deltaTime);
        }

        float distance = Vector3.Distance(transform.position, target.position);

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
            agent.SetDestination(transform.position);
            stateTimer = attackDelay;
            playedChargeAnim = false;
            currentState = MageState.PreparingAttack;
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
            agent.SetDestination(target.position);
            FaceTarget();
        }
    }

    private void HandlePreparingAttack(float distance)
    {
        FaceTarget();
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
            wasRecentlyHit = false; // Correction ici
            Attack();
            currentState = MageState.Attacking;
        }
    }

    private void HandleAttacking(float distance)
    {
        FaceTarget();
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
            Vector3 retreatDir = (transform.position - target.position).normalized;
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
        if (projectilePrefab == null || firePoint == null || target == null) return;

        // Calcul de la direction au moment du tir
        Vector3 direction = (target.position - firePoint.position).normalized;

        // Position légèrement décalée vers l'avant pour éviter de toucher le mage
        Vector3 spawnPos = firePoint.position + direction * 0.5f;

        // Instanciation du projectile orienté dans la bonne direction
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));

        // Initialisation de la trajectoire et des dégâts
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Initialize(direction, projectileSpeed, 10); // Remplace 10 par une variable si besoin
        }

        // Animation d'attaque
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

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        int hitType = Random.Range(1, 3); // 1 ou 2
        string triggerName = hitType == 1 ? "Hit1" : "Hit2";

        // Forcer la relance de l'animation
        animator.ResetTrigger("Hit1");
        animator.ResetTrigger("Hit2");
        animator.Play("Idle"); // ou un état neutre
        animator.SetTrigger(triggerName);

        stunCoroutine = StartCoroutine(HitReactionRoutine(hitType));
        StartCoroutine(FlashColor());
    }


    private IEnumerator HitReactionRoutine(int hitType)
    {
        isStunned = true;

        if (agent.isOnNavMesh)
            agent.isStopped = true;

        if (hitType == 2)
        {
            Vector3 dir = (transform.position - target.position).normalized;
            dir.y = 0f;

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

        yield return new WaitForSeconds(stunDuration);

        if (agent.isOnNavMesh) agent.isStopped = false;

        isStunned = false;
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

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
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

    private void FindTarget()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) target = obj.transform;
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

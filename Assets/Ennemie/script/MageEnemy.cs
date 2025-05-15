using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float retreatDuration = 1f;
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

    private NavMeshAgent agent;
    private Animator animator;
    private MageState currentState = MageState.Idle;
    private float stateTimer;
    private float lastAttackTime;
    private float lastRetreatTime;
    private float lastHitTime;
    private bool wasRecentlyHit = false;
    private Vector3 initialPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

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
            Debug.Log("Mage: Player detected, starting chase!");
            currentState = MageState.Chasing;
        }
    }

    private void HandleChasing(float distance)
    {
        if (distance <= attackRange)
        {
            agent.SetDestination(transform.position);
            stateTimer = attackDelay;
            Debug.Log("Mage: Preparing to attack!");
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
        stateTimer -= Time.deltaTime;
        FacePlayer();
        if (stateTimer <= 0f)
        {
            Debug.Log("Mage: Attack!");
            Attack();
            currentState = MageState.Attacking;
        }
    }

    private void HandleAttacking(float distance)
    {
        FacePlayer();

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (distance <= tooCloseDistance && Time.time - lastRetreatTime >= retreatCooldown)
            {
                Debug.Log("Mage: Player too close, preparing retreat!");
                stateTimer = 0.5f; // Wait before retreat
                currentState = MageState.PreparingRetreat;
            }
            else if (distance <= attackRange)
            {
                Debug.Log("Mage: Attack again!");
                stateTimer = attackDelay;
                currentState = MageState.PreparingAttack;
            }
            else if (distance > detectionRange)
            {
                Debug.Log("Mage: Player running away, chasing again!");
                currentState = MageState.Chasing;
            }
            else
            {
                Debug.Log("Mage: Player moved, chasing again!");
                currentState = MageState.Chasing;
            }
        }

        if (wasRecentlyHit && distance <= tooCloseDistance)
        {
            Debug.Log("Mage: Paralyzed by player attacks!");
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

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log("Mage: Retreating to safe position.");
                stateTimer = retreatDuration;
                lastRetreatTime = Time.time;
                currentState = MageState.Retreating;
            }
            else
            {
                Debug.LogWarning("Mage: Retreat path blocked, fallback to chase.");
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
            Debug.Log("Mage: Stopped retreat, reassess situation.");
            currentState = MageState.Attacking;
        }
    }

    private void HandleParalyzed()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            wasRecentlyHit = false;
            Debug.Log("Mage: No longer paralyzed, resuming fight!");
            currentState = MageState.Attacking;
        }
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

    public void ReceiveHit()
    {
        wasRecentlyHit = true;
        lastHitTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Hit");

        if (meshRenderer != null)
            StartCoroutine(FlashColor());
    }

    private IEnumerator FlashColor()
    {
        Color originalColor = meshRenderer.material.color;
        meshRenderer.material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        meshRenderer.material.color = originalColor;
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
        Gizmos.DrawWireSphere(transform.position, tooCloseDistance);
    }
}

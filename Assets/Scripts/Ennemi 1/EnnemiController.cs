using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemiController : MonoBehaviour
{
    public Transform[] patrolPoints; // Points de patrouille
    public int targetPoint;          // Indice du point actuel
    public float speed;              // Vitesse de déplacement
    public float stopDuration = 2f;  // Temps d'attente sur certains points
    public Transform player;         // Référence au joueur
    public float detectionRange = 10f;  // Distance à laquelle l'ennemi détecte le joueur
    public float attackRange = 2f;      // Distance à laquelle l'ennemi attaque le joueur
    public float attackCooldown = 1.5f; // Temps entre deux attaques

    private bool isWaiting = false;
    private bool isChasingPlayer = false;
    private bool isStunned = false; // Indique si l'ennemi est "stun"

    private float lastAttackTime;
    private Coroutine stunCoroutine; // Référence à la coroutine en cours pour réinitialiser le "stun"

    void Start()
    {
        targetPoint = 0;
    }

    void Update()
    {
        // Si l'ennemi est stun, il ne peut pas bouger ni attaquer
        if (isStunned) return;

        if (isWaiting) return;

        // Vérifie si le joueur est à portée de détection
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            isChasingPlayer = true;
        }
        else
        {
            isChasingPlayer = false;
        }

        // Comportement de poursuite ou de patrouille
        if (isChasingPlayer)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (transform.position == patrolPoints[targetPoint].position)
        {
            // Points spécifiques avec une pause
            if (targetPoint == 2 || targetPoint == 3 || targetPoint == 5)
            {
                StartCoroutine(StopAtWaypoint());
            }
            else
            {
                increaseTargetInt();
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, speed * Time.deltaTime);
    }

    void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Enemy attacks the player!");
            lastAttackTime = Time.time;

            // Infliger des dégâts au joueur
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // 1 point de dégât
            }
        }
    }

    void increaseTargetInt()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }

    IEnumerator StopAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(stopDuration);
        isWaiting = false;
        increaseTargetInt();
    }

    public void Stun(float duration)
    {
        // Si l'ennemi est déjà stun, réinitialise le chrono
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        Debug.Log("Enemy is stunned!");

        yield return new WaitForSeconds(duration);

        isStunned = false;
        Debug.Log("Enemy is no longer stunned.");
    }
}

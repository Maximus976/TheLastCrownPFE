using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemiController : MonoBehaviour
{
    public Transform[] patrolPoints; // Points de patrouille
    public int targetPoint;          // Indice du point actuel
    public float speed;              // Vitesse de d�placement
    public float stopDuration = 2f;  // Temps d'attente sur certains points
    public Transform player;         // R�f�rence au joueur
    public float detectionRange = 10f;  // Distance � laquelle l'ennemi d�tecte le joueur
    public float attackRange = 2f;      // Distance � laquelle l'ennemi attaque le joueur
    public float attackCooldown = 1.5f; // Temps entre deux attaques
    private bool isWaiting = false;
    private bool isChasingPlayer = false;
    private float lastAttackTime;

    void Start()
    {
        targetPoint = 0;
    }

    void Update()
    {
        if (isWaiting) return;

        // V�rifie si le joueur est � port�e de d�tection
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
            // Points sp�cifiques avec une pause
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

            // Infliger des d�g�ts au joueur
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // 1 point de d�g�t
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
}

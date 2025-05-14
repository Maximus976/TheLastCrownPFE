using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHealTrigger : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            // Appliquer l'effet de gu�rison sur le joueur
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.RestoreHealth(10); // Restaure 10 HP par exemple
            }
        }
    }
}
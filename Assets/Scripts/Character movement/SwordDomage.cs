using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collision détectée avec : {other.name}");

        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            Debug.Log($"Dégâts infligés à {other.name} via EnemyHealth.");
            enemyHealth.TakeDamage(damage);
        }

        var mannequin = other.GetComponent<Mannequin2>();
        if (mannequin != null)
        {
            Debug.Log($"Dégâts infligés à {other.name} via Mannequin2.");
            mannequin.TakeDamage(damage);
        }
    }
}

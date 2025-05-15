using System.Collections;
using UnityEngine;

public class Mannequin2 : MonoBehaviour
{
    public GameObject hitVFX;          // Effet visuel d'impact
    public GameObject destructionVFX; // Effet visuel de destruction
    public int maxHealth = 100;       // Points de vie maximum
    private int currentHealth;        // Points de vie actuels

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Mannequin a reçu {damage} dégâts.");
        currentHealth -= damage;

        if (hitVFX != null)
        {
            Instantiate(hitVFX, transform.position + Vector3.up, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Effet visuel de destruction
        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject damageSource;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == damageSource)
        {
            TakeDamage(40); // 40 points de dommages
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Point de vie ennemi: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy defeated!");
        Destroy(gameObject);
    }
}

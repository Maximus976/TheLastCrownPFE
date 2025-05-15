using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 5;  // Nombre maximum de vies
    private int currentLives; // Nombre actuel de vies

    void Start()
    {
        currentLives = maxLives;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentLives -= damage;
        Debug.Log($"Player hit! Lives remaining: {currentLives}");
        UpdateHealthUI();

        if (currentLives <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        // Ajouter ici la logique de mort (respawn, écran de fin, etc.)
    }

    private void UpdateHealthUI()
    {
        Debug.Log($"Updating health UI: {currentLives}/{maxLives}");
    }
}

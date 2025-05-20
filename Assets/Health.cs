using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("UI")]
    [SerializeField] private Image healthBarFill;  // Barre de vie à mettre à jour
    [SerializeField] private MenuMort menuMort;     // Script du menu de mort

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {currentHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        if (CinemachineShake.instance != null)
            CinemachineShake.instance.Shake();
    }

    public void RestoreHealth(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"{gameObject.name} healed {amount} HP. Current HP: {currentHealth}");

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"{gameObject.name} is dead.");

        // Ici tu peux désactiver le contrôle du joueur, l'animator, etc.

        if (menuMort != null)
        {
            menuMort.ActiverMenuMort(); // Affiche le canvas de mort
        }
        else
        {
            Debug.LogWarning("MenuMort n'est pas assigné !");
        }
    }
}
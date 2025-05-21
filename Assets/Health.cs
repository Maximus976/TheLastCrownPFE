using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    private int dieType;

    [Header("UI")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private MenuMort menuMort;
    [SerializeField] private GameObject hudUI;

    [Header("Dependencies")]
    [SerializeField] private Animator animator; // Ajout de référence à l'Animator
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnDeath; // Liste de scripts à désactiver à la mort

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

        if (CinemachineShake.instance != null)
            CinemachineShake.instance.Shake();

        if (currentHealth <= 0)
        {
            Die();
        }
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

        // Choisir une animation de mort aléatoire entre 1 et 4
        dieType = Random.Range(1, 5); // 5 est exclusif → [1, 4]
        if (animator != null)
        {
            animator.SetInteger("DieType", dieType);
            animator.SetTrigger("Die");
        }

        // Désactiver le HUD
        if (hudUI != null)
        {
            hudUI.SetActive(false);
        }

        // Désactiver les scripts spécifiés
        foreach (var script in scriptsToDisableOnDeath)
        {
            if (script != null)
                script.enabled = false;
        }

        // Afficher le menu de mort
        if (menuMort != null)
        {
            menuMort.ActiverMenuMort();
        }
        else
        {
            Debug.LogWarning("MenuMort n'est pas assigné !");
        }
    }

}

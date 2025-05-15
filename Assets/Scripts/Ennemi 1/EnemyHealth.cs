using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private Image healthBarFill; // optionnel, pour UI

    private bool isDead = false;

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
        UpdateHealthBar();

        // Réagir au coup
        EnemyMovement movement = GetComponentInChildren<EnemyMovement>();
        if (movement != null && GameObject.FindWithTag("Player") != null)
        {
            Vector3 attackerPos = GameObject.FindWithTag("Player").transform.position;
            movement.ReactToHit(attackerPos);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        EnemyMovement movement = GetComponentInChildren<EnemyMovement>();
        if (movement != null)
        {
            movement.Die();
        }

        StartCoroutine(DelayedCleanup());
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private IEnumerator DelayedCleanup()
    {
        yield return new WaitForSeconds(1.5f);

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        this.enabled = false;
        Destroy(gameObject, 2f);
    }
}

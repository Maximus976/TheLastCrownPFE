using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private Image healthBarFill;

    private bool isDead = false;

    private MageEnemy mage;
    private EnemyMovement standardAI;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Caches optionnels
        mage = GetComponent<MageEnemy>();
        standardAI = GetComponentInChildren<EnemyMovement>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        // === Spécifique Mage ===
        if (mage != null)
        {
            mage.ReceiveHit(); // Feedback mage
        }

        // === Ennemi standard ===
        else if (standardAI != null && GameObject.FindWithTag("Player") != null)
        {
            Vector3 attackerPos = GameObject.FindWithTag("Player").transform.position;
            standardAI.ReactToHit(attackerPos);
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

        // === Mage ===
        if (mage != null)
        {
            Destroy(gameObject, 2f); // Mage auto-détruit, géré via ses anims
        }

        // === Ennemi standard ===
        else if (standardAI != null)
        {
            standardAI.Die();
            StartCoroutine(DelayedCleanup());
        }
        else
        {
            Destroy(gameObject, 2f);
        }
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

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private Image healthBarFill;

    private bool isDead = false;
    public bool IsDead => isDead;

    private MageEnemy mage;
    private EnemyMovement standardAI;
    private Animator animator;

    [Header("Boss Settings")]
    [SerializeField] private bool isBoss = false; // ✅ À cocher dans l'Inspector pour le boss

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        mage = GetComponent<MageEnemy>();
        standardAI = GetComponentInChildren<EnemyMovement>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (mage != null)
        {
            mage.ReceiveHit();
        }
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

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.SetTrigger("Die");
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (standardAI != null)
        {
            standardAI.Die();
        }

        // ✅ Si c’est un boss, on lance la séquence de fin
        if (isBoss)
        {
            Debug.Log("Boss vaincu – lancement de la séquence de fin");
            FindObjectOfType<FinNarrative>()?.StartFinSequence();
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
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
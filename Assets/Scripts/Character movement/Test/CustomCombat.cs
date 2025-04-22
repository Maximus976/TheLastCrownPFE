using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public Animator animator;
    public Rigidbody rb;
    [SerializeField] private float attackDashSpeed = 4f;
    [SerializeField] private float attackDashTime = 0.1f;
    [SerializeField] private float comboResetTime = 1.5f;
    [SerializeField] private float comboLockTime = 0.5f;
    [SerializeField] private float comboEndDelay = 2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Attack Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float hitRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private Coroutine currentAttackCoroutine;
    private Camera mainCamera;

    private static readonly int IsInCombat = Animator.StringToHash("IsInCombat");

    public bool IsAttacking => isAttacking;

    void Start()
    {
        if (animator == null) Debug.LogError("Animator not assigned!");
        if (rb == null) Debug.LogError("Rigidbody not assigned!");

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (IsAttacking) return;

        float resetDelay = (comboStep == 0) ? comboResetTime : (comboStep == 2 ? comboEndDelay : comboResetTime);

        if (Time.time - lastAttackTime > resetDelay)
        {
            comboStep = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RotateTowardMouse();
            StartLightAttack();
        }
    }

    private void RotateTowardMouse()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            Vector3 direction = hitInfo.point - transform.position;
            direction.y = 0;
            transform.forward = direction;
        }
    }

    public void StartLightAttack()
    {
        if (isAttacking) return;

        animator.SetBool(IsInCombat, true);

        if (currentAttackCoroutine != null)
            StopCoroutine(currentAttackCoroutine);
        currentAttackCoroutine = StartCoroutine(PerformAttack(light: true));
    }

    private IEnumerator PerformAttack(bool light)
    {
        isAttacking = true;

        if (light)
        {
            switch (comboStep)
            {
                case 0:
                    animator.SetTrigger("Hit 1");
                    comboStep = 1;
                    break;
                case 1:
                    animator.SetTrigger("Hit 2");
                    comboStep = 2;
                    break;
                case 2:
                    animator.SetTrigger("Hit 3");
                    comboStep = 0;
                    break;
            }
        }

        lastAttackTime = Time.time;

        // Petit dash vers la direction du regard
        float dashEndTime = Time.time + attackDashTime;
        Vector3 dashDirection = transform.forward;
        while (Time.time < dashEndTime)
        {
            rb.MovePosition(rb.position + dashDirection * attackDashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        // ✅ On vérifie les ennemis à toucher
        CheckForEnemiesHit();

        yield return new WaitForSeconds(comboLockTime);

        isAttacking = false;
        animator.SetBool(IsInCombat, false);
    }

    private void CheckForEnemiesHit()
    {
        Vector3 center = transform.position + transform.forward * 1f;
        Collider[] hits = Physics.OverlapSphere(center, hitRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            Health enemyHealth = hit.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ➕ Visualisation du rayon de frappe
        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * 1f;
        Gizmos.DrawWireSphere(center, hitRange);
    }
}
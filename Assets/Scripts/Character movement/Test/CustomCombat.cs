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
    [SerializeField] private float attackAnimDuration = 0.5f;
    [SerializeField] private float comboResetTime = 1.5f;
    [SerializeField] private float comboLockTime = 0.5f;
    [SerializeField] private float comboEndDelay = 2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Attack Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float hitRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("VFX Settings")]
    [SerializeField] private GameObject attackVFXPrefab;
    [SerializeField] private Transform vfxSpawnPoint;
    [SerializeField] private float vfxSpawnDelay = 0.1f;

    [Header("Weapon")]
    [SerializeField] private Transform swordTransform;

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private Coroutine currentAttackCoroutine;
    private Camera mainCamera;

    private Vector3 lastMouseDirection = Vector3.forward;

    private static readonly int IsInCombat = Animator.StringToHash("IsInCombat");

    public bool IsAttacking => isAttacking;

    void Start()
    {
        if (animator == null) Debug.LogError("Animator not assigned!");
        if (rb == null) Debug.LogError("Rigidbody not assigned!");
        if (vfxSpawnPoint == null) Debug.LogError("VFX Spawn Point not assigned!");
        if (swordTransform == null) Debug.LogWarning("Sword Transform not assigned!");

        mainCamera = Camera.main;
    }

    void Update()
    {
        float resetDelay = (comboStep == 0) ? comboResetTime : (comboStep == 2 ? comboEndDelay : comboResetTime);

        if (Time.time - lastAttackTime > resetDelay)
        {
            comboStep = 0;
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1")) && !IsAttacking)
        {
            RotateTowardMouseInstant();
            StartLightAttack();
        }
    }

    private void RotateTowardMouseInstant()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 1f);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            Vector3 direction = hitInfo.point - transform.position;
            direction.y = 0;
            direction.Normalize();

            lastMouseDirection = direction;
        }
        else
        {
            Debug.Log("Raycast souris n'a rien touché !");
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

        // Rotation physique avant d'attaquer
        Quaternion targetRotation = Quaternion.LookRotation(lastMouseDirection);
        rb.MoveRotation(targetRotation);
        yield return new WaitForFixedUpdate(); // Laisse Unity appliquer la rotation

        Vector3 dashDirection = transform.forward;
        Quaternion vfxRotation = Quaternion.LookRotation(dashDirection);

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
                    vfxRotation *= Quaternion.Euler(0, 0, 90f);
                    break;
            }

            // Applique une rotation de l’épée à chaque attaque
            if (swordTransform != null)
            {
                swordTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                StartCoroutine(ResetSwordRotationAfterDelay(0.1f));
            }
        }

        lastAttackTime = Time.time;

        float dashEndTime = Time.time + attackDashTime;
        while (Time.time < dashEndTime)
        {
            rb.MovePosition(rb.position + dashDirection * attackDashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        if (attackVFXPrefab != null && vfxSpawnPoint != null)
        {
            yield return new WaitForSeconds(vfxSpawnDelay);
            Instantiate(attackVFXPrefab, vfxSpawnPoint.position, vfxRotation);
        }

        CheckForEnemiesHit();

        float endTime = Time.time + attackAnimDuration;
        while (Time.time < endTime)
            yield return null;

        isAttacking = false;
        animator.SetBool(IsInCombat, false);
    }

    private IEnumerator ResetSwordRotationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (swordTransform != null)
            swordTransform.localRotation = Quaternion.identity;
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
                continue;
            }

            Destructible destructible = hit.GetComponent<Destructible>();
            if (destructible != null)
            {
                destructible.TakeDamage(damageAmount);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * 1f;
        Gizmos.DrawWireSphere(center, hitRange);
    }
}
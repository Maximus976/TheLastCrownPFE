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
    private bool usingGamepad = false;

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
        DetectInputMethod();

        float resetDelay = (comboStep == 0) ? comboResetTime : (comboStep == 2 ? comboEndDelay : comboResetTime);

        if (Time.time - lastAttackTime > resetDelay)
        {
            comboStep = 0;
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1")) && !IsAttacking)
        {
            if (!usingGamepad)
            {
                RotateTowardMouseInstant();
            }
            else
            {
                RotateTowardStick();
            }

            StartLightAttack();
        }
    }

    private void DetectInputMethod()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            usingGamepad = false;
        }

        if (Input.GetButtonDown("Fire1") || Mathf.Abs(Input.GetAxis("RightStickHorizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("RightStickVertical")) > 0.1f)
        {
            usingGamepad = true;
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

    private void RotateTowardStick()
    {
        float rightX = Input.GetAxis("RightStickHorizontal");
        float rightY = Input.GetAxis("RightStickVertical");

        Vector3 input = new Vector3(rightX, 0f, rightY);
        if (input.magnitude > 0.1f)
        {
            Transform cam = Camera.main.transform;
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 aimDir = camForward * rightY + camRight * rightX;
            lastMouseDirection = aimDir.normalized;
        }
        else
        {
            lastMouseDirection = transform.forward;
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

        Quaternion targetRotation = Quaternion.LookRotation(lastMouseDirection);
        rb.MoveRotation(targetRotation);
        yield return new WaitForFixedUpdate();

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

            if (swordTransform != null)
            {
                swordTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                StartCoroutine(ResetSwordRotationAfterDelay(0.5f));
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
        Vector3 center = transform.position + transform.forward * 2f;
        Collider[] hits = Physics.OverlapSphere(center, hitRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
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
        Vector3 center = transform.position + transform.forward * 2f;
        Gizmos.DrawWireSphere(center, hitRange);
    }
}

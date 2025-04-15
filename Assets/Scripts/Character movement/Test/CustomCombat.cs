using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCombat : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public MonoBehaviour movementScript; // CustomMovement
    public MonoBehaviour followScript;   // PlayerFollowMouss

    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    public float attackDelay = 0.5f;
    public float maxComboDelay = 1.5f;
    public float chargeThreshold = 0.7f;
    public float movementDisableDuration = 0.5f;
    public float attackDashSpeed = 8f;
    public float attackDashTime = 0.1f;

    private float chargeStartTime;
    private bool isCharging = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private int comboStep = 0;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!animator) Debug.LogError("Animator is missing!");
    }

    void Update()
    {
        HandleChargeLogic();
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("▶️ StartPrepare envoyé");
            animator.SetTrigger("StartPrepare");
        }

    }

    private void HandleChargeLogic()
    {
        if (Input.GetMouseButtonDown(0) && !isCharging && !isAttacking)
        {
            isCharging = true;
            chargeStartTime = Time.time;
            animator.SetTrigger("StartPrepare");

            if (movementScript) movementScript.enabled = false;
            if (followScript) followScript.enabled = true;
        }

        if (isCharging && Time.time - chargeStartTime >= chargeThreshold)
        {
            animator.SetBool("IsChargingHeavy", true);
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            float heldTime = Time.time - chargeStartTime;
            animator.SetBool("IsChargingHeavy", false);

            if (heldTime >= chargeThreshold)
            {
                animator.SetTrigger("HeavyAttack");
                StartCoroutine(ExecuteAttack());
            }
            else if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("LightAttack");
                StartCoroutine(PerformCombo());
            }
        }
    }

    private IEnumerator ExecuteAttack()
    {
        isAttacking = true;

        Vector3 direction = transform.forward;
        float dashEndTime = Time.time + attackDashTime;

        while (Time.time < dashEndTime)
        {
            rb.MovePosition(rb.position + direction * attackDashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(attackDelay);
        yield return new WaitForSeconds(movementDisableDuration);

        if (movementScript) movementScript.enabled = true;
        if (followScript) followScript.enabled = false;

        isAttacking = false;
    }

    private IEnumerator PerformCombo()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (comboStep == 0)
        {
            animator.SetTrigger("Hit 1");
            comboStep = 1;
        }
        else if (comboStep == 1 && Time.time - lastAttackTime <= maxComboDelay)
        {
            animator.SetTrigger("Hit 2");
            comboStep = 2;
        }
        else if (comboStep == 2 && Time.time - lastAttackTime <= maxComboDelay)
        {
            animator.SetTrigger("Hit 3");
            comboStep = 0;
        }
        else
        {
            comboStep = 0;
            animator.SetTrigger("Hit 1");
        }

        yield return new WaitForSeconds(attackDelay);
        yield return new WaitForSeconds(movementDisableDuration);

        if (movementScript) movementScript.enabled = true;
        if (followScript) followScript.enabled = false;

        isAttacking = false;
    }
}


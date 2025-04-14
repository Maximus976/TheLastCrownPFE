using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCombat : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public MonoBehaviour movementScript; // CustomMovement
    public MonoBehaviour followScript;   // PlayerFollowMouss

    [Header("Attack")]
    public float attackCooldown = 1f;
    public float attackDelay = 0.5f;
    public float maxComboDelay = 1.5f;
    [SerializeField] private float attackDashSpeed = 8f;
    [SerializeField] private float attackDashTime = 0.1f;

    [Header("Charged Attack")]
    [SerializeField] private float chargeThreshold = 0.7f;

    [Header("Settings")]
    [SerializeField] private float movementDisableDuration = 1f;

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
    }

    private void HandleChargeLogic()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && !isCharging)
        {
            isCharging = true;
            chargeStartTime = Time.time;
            animator.SetBool("IsCharging", true);
            rb.velocity = Vector3.zero;

            if (movementScript) movementScript.enabled = false;
            if (followScript) followScript.enabled = true;
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            animator.SetBool("IsCharging", false);

            float heldTime = Time.time - chargeStartTime;

            if (heldTime >= chargeThreshold)
                StartCoroutine(PerformChargedAttack());
            else if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                StartCoroutine(PerformAttack());
            else
                StartCoroutine(ReenableMovementDelayed());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

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

        lastAttackTime = Time.time;

        Vector3 attackDirection = transform.forward;
        float dashEndTime = Time.time + attackDashTime;

        while (Time.time < dashEndTime)
        {
            rb.MovePosition(rb.position + attackDirection * attackDashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(attackDelay);
        StartCoroutine(ReenableMovementDelayed());
        isAttacking = false;
    }

    private IEnumerator PerformChargedAttack()
    {
        isAttacking = true;
        animator.SetTrigger("ChargeAttack");

        Vector3 attackDirection = transform.forward;
        float dashEndTime = Time.time + attackDashTime;

        while (Time.time < dashEndTime)
        {
            rb.MovePosition(rb.position + attackDirection * attackDashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(attackDelay);
        StartCoroutine(ReenableMovementDelayed());
        isAttacking = false;
    }

    private IEnumerator ReenableMovementDelayed()
    {
        yield return new WaitForSeconds(movementDisableDuration);
        if (movementScript) movementScript.enabled = true;
        if (followScript) followScript.enabled = false;
    }
}


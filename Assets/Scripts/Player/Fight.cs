using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fight : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    public Animator animator;

    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    public float attackDelay = 0.5f;
    public float attackCooldown = 1f;
    public float maxComboDelay = 1.5f;

    private Rigidbody rb;
    private Vector3 moveDirection;

    private bool isDashing = false;
    private bool isAttacking = false;

    private float lastAttackTime = 0f;
    private int comboStep = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (animator == null)
        {
            Debug.LogError("Animator component is missing on this GameObject!");
        }
    }

    void Update()
    {
        if (!isDashing && !isAttacking)
        {
            HandleMovement();
        }

        if (UnityEngine.Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.X) && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    void FixedUpdate()
    {
        if (!isDashing && !isAttacking)
        {
            rb.velocity = moveDirection * speed;
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (UnityEngine.Input.GetKey(KeyCode.W)) moveVertical += 1f;   // Haut
        if (UnityEngine.Input.GetKey(KeyCode.S)) moveVertical -= 1f;   // Bas
        if (UnityEngine.Input.GetKey(KeyCode.A)) moveHorizontal -= 1f; // Gauche
        if (UnityEngine.Input.GetKey(KeyCode.D)) moveHorizontal += 1f; // Droite

        Vector3 localDirection = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        Transform cameraTransform = Camera.main.transform;
        Vector3 forward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
        moveDirection = localDirection.z * forward + localDirection.x * right;

        if (moveDirection.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator PerformAttack()
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

        animator.ResetTrigger("Hit 1");
        animator.ResetTrigger("Hit 2");
        animator.ResetTrigger("Hit 3");
        isAttacking = false;
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;

        animator.SetTrigger("Dash");

        Vector3 dashDirection = moveDirection;

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
    }
}



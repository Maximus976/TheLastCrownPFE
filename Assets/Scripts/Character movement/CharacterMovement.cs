using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.Searcher.SearcherWindow.Alignment;


public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    public Animator animator;

    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    public float attackDelay = 0.5f;
    public float attackCooldown = 1f;

    private Rigidbody rb;
    private Vector3 moveDirection;

    private bool isDashing = false;
    private bool canDash = true;

    private bool isAttacking = false;
    private float lastAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!isDashing && !isAttacking)
        {
            HandleMovement();
        }

        if (UnityEngine.Input.GetMouseButtonDown(0) && !isDashing && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.X) && !isDashing && canDash)
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

        if (UnityEngine.Input.GetKey(KeyCode.W)) moveVertical += 1f;   // Avancer
        if (UnityEngine.Input.GetKey(KeyCode.S)) moveVertical -= 1f;   // Reculer
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
            animator.SetFloat("MoveX", localDirection.x);
            animator.SetFloat("MoveZ", localDirection.z);
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

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        isAttacking = false;
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        animator.SetTrigger("Dash");

        Vector3 dashDirection = moveDirection;

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector3.zero; // Arrêt du dash
        isDashing = false;

        // Temps de recharge du dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}

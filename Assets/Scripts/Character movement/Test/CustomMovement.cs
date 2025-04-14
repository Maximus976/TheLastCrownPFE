using System.Collections;
using UnityEngine;

public class CustomMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    private bool isSprinting = false;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashRecoveryTime = 0.3f;
    private bool dashOnCooldown = false;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isDashing = false;

    private float inputTapTimer = 0f;
    private float tapDuration = 0.1f;
    private bool wasHoldingMovement = false;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (animator == null)
            Debug.LogError("Animator component is missing!");

        animator.SetFloat("MoveSpeed", 0f);
        animator.SetFloat("IsStrafing", 0f);
        animator.SetFloat("ForwardStrafe", 0f);
        animator.SetFloat("StrafeDirectionX", 0f);
        animator.SetFloat("StrafeDirectionZ", 0f);
        animator.SetFloat("InclineAngle", 0f);
    }

    void Update()
    {
        HandleInput();

        if (inputTapTimer > 0f)
        {
            inputTapTimer -= Time.deltaTime;
            if (inputTapTimer <= 0f)
                animator.SetBool("MovementInputTapped", false);
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        float currentSpeed = isSprinting ? sprintMultiplier : 1f;
        rb.velocity = new Vector3(moveDirection.x * speed * currentSpeed, rb.velocity.y, moveDirection.z * speed * currentSpeed);
    }

    private void HandleInput()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.C) && !isDashing && moveDirection.magnitude > 0.1f)
        {
            StartCoroutine(PerformDash());
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey("w")) moveVertical += 1f;
        if (Input.GetKey("s")) moveVertical -= 1f;
        if (Input.GetKey("a")) moveHorizontal -= 1f;
        if (Input.GetKey("d")) moveHorizontal += 1f;

        Vector3 localDirection = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        if (localDirection.magnitude == 0f)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            Transform cam = Camera.main.transform;
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            moveDirection = localDirection.z * forward + localDirection.x * right;

            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        bool holdingMovement = moveDirection.magnitude > 0.1f;
        isSprinting = Input.GetKey(KeyCode.LeftShift) && holdingMovement;
        float speedMultiplier = isSprinting ? sprintMultiplier : 1f;

        if (!wasHoldingMovement && holdingMovement)
        {
            animator.SetBool("MovementInputTapped", true);
            inputTapTimer = tapDuration;
        }
        wasHoldingMovement = holdingMovement;

        float currentSpeed = animator.GetFloat("MoveSpeed");
        float targetSpeed = holdingMovement ? 1f * speedMultiplier : 0f;
        float smoothedSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

        animator.SetBool("MovementInputHeld", holdingMovement);
        animator.SetBool("IsStopped", !holdingMovement);
        animator.SetBool("IsGrounded", true);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsStarting", false);

        animator.SetFloat("MoveSpeed", smoothedSpeed);
        animator.SetFloat("IsStrafing", 0f);
        animator.SetFloat("ForwardStrafe", holdingMovement ? speedMultiplier : 0f);
        animator.SetFloat("StrafeDirectionX", 0f);
        animator.SetFloat("StrafeDirectionZ", 0f);
        animator.SetFloat("InclineAngle", 0f);
    }

    private IEnumerator PerformDash()
    {
        if (dashOnCooldown) yield break;

        isDashing = true;
        dashOnCooldown = true;
        animator.SetTrigger("Slide");

        Vector3 dashDirection = moveDirection.magnitude > 0.1f ? moveDirection.normalized : transform.forward;
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(dashRecoveryTime);

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }
}

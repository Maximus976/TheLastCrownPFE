using System.Collections;
using UnityEngine;

public class CustomMovement : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 3f;              // Course lente (joystick léger)
    public float fullSpeed = 5f;              // Course normale
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashRecoveryTime = 0.3f;

    private bool isSprinting = false;
    private bool dashOnCooldown = false;
    private bool isDashing = false;
    private Vector3 currentDashDirection;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float inputTapTimer = 0f;
    private float tapDuration = 0.1f;
    private bool wasHoldingMovement = false;

    public Animator animator;
    private CustomCombat combatScript;

    private float targetSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (animator == null)
            Debug.LogError("Animator component is missing!");

        combatScript = GetComponent<CustomCombat>();
    }

    void Update()
    {
        if (combatScript != null && combatScript.IsAttacking) return;

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
        if (isDashing || (combatScript != null && combatScript.IsAttacking))
            return;

        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocity = moveDirection.normalized * targetSpeed;
        rb.velocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
    }

    private void HandleInput()
    {
        HandleMovement();

        if ((Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Dash")) && !isDashing && moveDirection.magnitude > 0.1f)
        {
            StartCoroutine(PerformDash());
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 localInput = new Vector3(moveHorizontal, 0, moveVertical);
        float inputMagnitude = Mathf.Clamp01(localInput.magnitude);

        Transform cam = Camera.main.transform;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = forward * moveVertical + right * moveHorizontal;

        bool hasInput = inputMagnitude > 0.1f;

        // ROTATION
        if (hasInput && !combatScript.IsAttacking)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        // SPRINT
        bool wantsToSprint = (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint")) && inputMagnitude > 0.9f;

        if (wantsToSprint)
        {
            targetSpeed = fullSpeed * sprintMultiplier;
            isSprinting = true;
        }
        else if (inputMagnitude > 0.6f)
        {
            targetSpeed = fullSpeed;
            isSprinting = false;
        }
        else if (inputMagnitude > 0.1f)
        {
            targetSpeed = baseSpeed;
            isSprinting = false;
        }
        else
        {
            targetSpeed = 0f;
            isSprinting = false;
        }

        // ANIMATIONS
        if (!wasHoldingMovement && hasInput)
        {
            animator.SetBool("MovementInputTapped", true);
            inputTapTimer = tapDuration;
        }
        wasHoldingMovement = hasInput;

        float currentAnimSpeed = animator.GetFloat("MoveSpeed");

        float normalizedSpeed = 0f;
        if (targetSpeed > 0f)
            normalizedSpeed = Mathf.Clamp01(targetSpeed / (fullSpeed * sprintMultiplier)); // Normalize to max sprint speed

        float smoothedSpeed = Mathf.Lerp(currentAnimSpeed, normalizedSpeed, Time.deltaTime * 10f);

        animator.SetBool("MovementInputHeld", hasInput);
        animator.SetBool("IsStopped", !hasInput);
        animator.SetBool("IsGrounded", true);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsStarting", false);

        animator.SetFloat("MoveSpeed", smoothedSpeed);
        animator.SetFloat("ForwardStrafe", hasInput ? normalizedSpeed : 0f);
        animator.SetFloat("StrafeDirectionX", 0f);
        animator.SetFloat("StrafeDirectionZ", 0f);
        animator.SetFloat("InclineAngle", 0f);
    }

    private IEnumerator PerformDash()
    {
        if (dashOnCooldown) yield break;

        dashOnCooldown = true;
        animator.SetTrigger("Slide");

        Vector3 rawDir = moveDirection.magnitude > 0.1f ? moveDirection : transform.forward;
        rawDir.y = 0f;
        currentDashDirection = rawDir.normalized;

        isDashing = true;
        rb.velocity = currentDashDirection * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(dashRecoveryTime);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    private void OnDrawGizmos()
    {
        if (!isDashing) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + currentDashDirection * 2f);
        Gizmos.DrawSphere(transform.position + currentDashDirection * 2f, 0.1f);
    }
}

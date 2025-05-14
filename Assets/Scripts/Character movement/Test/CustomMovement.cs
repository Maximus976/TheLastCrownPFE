using System.Collections;
using UnityEngine;

public class CustomMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    [SerializeField] private float walkMultiplier = 0.5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float sprintBoostDelay = 1.5f;

    private bool isSprinting = false;
    private bool isBoostingSprint = false;
    private float runDuration = 0f;
    private float targetSpeedMultiplier = 1f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashRecoveryTime = 0.3f;
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (animator == null)
            Debug.LogError("Animator component is missing!");

        animator.SetFloat("MoveSpeed", 0f);
        animator.SetFloat("IsStrafing", 1f);
        animator.SetFloat("ForwardStrafe", 0f);
        animator.SetFloat("StrafeDirectionX", 0f);
        animator.SetFloat("StrafeDirectionZ", 0f);

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
        Vector3 targetVelocity = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized * speed * targetSpeedMultiplier;
        Vector3 smoothedVelocity = Vector3.Lerp(new Vector3(currentVelocity.x, 0f, currentVelocity.z), targetVelocity, Time.fixedDeltaTime * 10f);
        rb.velocity = new Vector3(smoothedVelocity.x, currentVelocity.y, smoothedVelocity.z);
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

        Vector3 localDirection = new Vector3(moveHorizontal, 0, moveVertical);

        if (localDirection.magnitude < 0.1f)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            localDirection.Normalize();

            Transform cam = Camera.main.transform;
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            moveDirection = localDirection.z * forward + localDirection.x * right;

            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            if (!combatScript.IsAttacking)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        bool holdingMovement = moveDirection.magnitude > 0.1f;
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint");

        if (holdingMovement)
        {
            runDuration += Time.deltaTime;
        }
        else
        {
            runDuration = 0f;
        }

        isSprinting = shiftHeld && holdingMovement;
        isBoostingSprint = isSprinting;

        if (isSprinting)
        {
            targetSpeedMultiplier = sprintMultiplier;
        }
        else if (runDuration < sprintBoostDelay)
        {
            targetSpeedMultiplier = walkMultiplier;
        }
        else
        {
            targetSpeedMultiplier = 1f;
        }

        if (!wasHoldingMovement && holdingMovement)
        {
            animator.SetBool("MovementInputTapped", true);
            inputTapTimer = tapDuration;
        }
        wasHoldingMovement = holdingMovement;

        float forwardStrafe = 0f;
        float targetSpeed = 0f;
        if (holdingMovement)
        {
            if (isSprinting)
            {
                forwardStrafe = 4f;
            }
            else if (runDuration >= sprintBoostDelay)
            {
                forwardStrafe = 2.5f;
            }
            else
            {
                forwardStrafe = 1.4f;
            }
            targetSpeed = forwardStrafe;
        }

        animator.SetBool("MovementInputHeld", holdingMovement);
        animator.SetBool("IsStopped", !holdingMovement);
        animator.SetBool("IsGrounded", true);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsStarting", false);
        animator.SetBool("MovementInputTapped", false);

        float currentForward = animator.GetFloat("ForwardStrafe");
        float currentMoveSpeed = animator.GetFloat("MoveSpeed");

        float smoothing = holdingMovement ? 10f : 3f;

        float smoothedForward = Mathf.Lerp(currentForward, forwardStrafe, Time.deltaTime * smoothing);
        float smoothedSpeed = Mathf.Lerp(currentMoveSpeed, targetSpeed, Time.deltaTime * smoothing);

        animator.SetFloat("ForwardStrafe", smoothedForward);
        animator.SetFloat("MoveSpeed", smoothedSpeed);
        animator.SetFloat("IsStrafing", 1f);

        Vector3 localMoveDir = transform.InverseTransformDirection(moveDirection);
        Vector3 currentDir = new Vector3(
            animator.GetFloat("StrafeDirectionX"),
            0f,
            animator.GetFloat("StrafeDirectionZ")
        );
        Vector3 smoothedDir = Vector3.Lerp(currentDir, new Vector3(localMoveDir.x, 0, localMoveDir.z), Time.deltaTime * smoothing);

        animator.SetFloat("StrafeDirectionX", smoothedDir.x);
        animator.SetFloat("StrafeDirectionZ", smoothedDir.z);
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

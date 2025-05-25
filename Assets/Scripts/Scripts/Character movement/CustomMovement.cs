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

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashRecoveryTime = 0.3f;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip walkFootstep;
    [SerializeField] private AudioClip runFootstep;
    [SerializeField] private float walkStepRate = 0.6f;
    [SerializeField] private float runStepRate = 0.35f;

    public Animator animator;

    private bool isSprinting = false;
    private float runDuration = 0f;
    private float targetSpeedMultiplier = 1f;
    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private Vector3 currentDashDirection;

    private Rigidbody rb;
    private Vector3 moveDirection;

    private bool wasHoldingMovement = false;
    private float inputTapTimer = 0f;
    private float tapDuration = 0.1f;

    private float footstepTimer = 0f;

    private CustomCombat combatScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        combatScript = GetComponent<CustomCombat>();

        animator.SetFloat("MoveSpeed", 0f);
        animator.SetFloat("IsStrafing", 1f);
        animator.SetFloat("ForwardStrafe", 0f);
        animator.SetFloat("StrafeDirectionX", 0f);
        animator.SetFloat("StrafeDirectionZ", 0f);
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
        Vector3 targetVelocity = moveDirection.normalized * speed * targetSpeedMultiplier;
        Vector3 smoothedVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * 10f);
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

        Vector3 localDirection = new Vector3(moveHorizontal, 0f, moveVertical);

        if (localDirection.magnitude < 0.1f)
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

            moveDirection = (localDirection.z * forward + localDirection.x * right).normalized;

            if (!combatScript.IsAttacking)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        bool holdingMovement = moveDirection.magnitude > 0.1f;
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint");

        isSprinting = shiftHeld && holdingMovement;

        targetSpeedMultiplier = isSprinting ? sprintMultiplier :
                                runDuration < sprintBoostDelay ? walkMultiplier : 1f;

        if (holdingMovement)
            runDuration += Time.deltaTime;
        else
            runDuration = 0f;

        if (!wasHoldingMovement && holdingMovement)
        {
            animator.SetBool("MovementInputTapped", true);
            inputTapTimer = tapDuration;
        }

        wasHoldingMovement = holdingMovement;

        float forwardStrafe = 0f;
        if (holdingMovement)
        {
            forwardStrafe = isSprinting ? 4f : runDuration >= sprintBoostDelay ? 2.5f : 1.4f;
        }

        animator.SetBool("MovementInputHeld", holdingMovement);
        animator.SetBool("IsStopped", !holdingMovement);
        animator.SetFloat("MoveSpeed", Mathf.Lerp(animator.GetFloat("MoveSpeed"), forwardStrafe, Time.deltaTime * 10f));
        animator.SetFloat("ForwardStrafe", Mathf.Lerp(animator.GetFloat("ForwardStrafe"), forwardStrafe, Time.deltaTime * 10f));

        Vector3 localMoveDir = transform.InverseTransformDirection(moveDirection);
        Vector3 smoothedDir = Vector3.Lerp(
            new Vector3(animator.GetFloat("StrafeDirectionX"), 0f, animator.GetFloat("StrafeDirectionZ")),
            new Vector3(localMoveDir.x, 0, localMoveDir.z),
            Time.deltaTime * 10f
        );

        animator.SetFloat("StrafeDirectionX", smoothedDir.x);
        animator.SetFloat("StrafeDirectionZ", smoothedDir.z);
        animator.SetFloat("IsStrafing", 1f);

        HandleFootsteps(holdingMovement, isSprinting);
    }

    private void HandleFootsteps(bool isMoving, bool isSprinting)
    {
        if (!isMoving || isDashing || combatScript.IsAttacking) return;

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            footstepSource.clip = isSprinting ? runFootstep : walkFootstep;
            footstepSource.Play();

            footstepTimer = isSprinting ? runStepRate : walkStepRate;
        }
    }

    private IEnumerator PerformDash()
    {
        if (dashOnCooldown) yield break;

        dashOnCooldown = true;
        animator.SetTrigger("Slide");

        Vector3 rawDir = moveDirection.magnitude > 0.1f ? moveDirection : transform.forward;
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
}
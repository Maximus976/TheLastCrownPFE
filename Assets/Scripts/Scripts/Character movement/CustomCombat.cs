using System.Collections;
using UnityEngine;

public class CustomCombat : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb;

    [SerializeField] private float attackDashSpeed = 4f;
    [SerializeField] private float attackDashTime = 0.1f;
    [SerializeField] private float attackAnimDuration = 0.4f;
    [SerializeField] private float hitRange = 1.5f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject[] attackVFXPrefabs;
    [SerializeField] private Transform vfxSpawnPoint;
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private float attackInputCooldown = 0.2f;

    private int comboStep = 0;
    private bool isPerformingAttack = false;
    private bool attackQueued = false;
    private float lastAttackInputTime = -999f;

    private Coroutine currentAttackCoroutine;
    private Camera mainCamera;
    private Vector3 lastMouseDirection = Vector3.forward;
    private AudioSource audioSource;
    private bool usingGamepad = false;

    public bool IsAttacking => isPerformingAttack;

    void Start()
    {
        mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void Update()
    {
        DetectInputDevice();

        if ((Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1")))
        {
            if (Time.time - lastAttackInputTime < attackInputCooldown)
                return;

            lastAttackInputTime = Time.time;

            if (!usingGamepad)
            {
                RotateTowardMouseInstant();
            }

            if (isPerformingAttack)
            {
                attackQueued = true;
            }
            else
            {
                StartAttackSequence();
            }
        }
    }

    private void DetectInputDevice()
    {
        // Si la souris bouge → clavier/souris
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            usingGamepad = false;
        }

        // Si le stick gauche bouge → manette
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            usingGamepad = true;
        }
    }

    private void RotateTowardMouseInstant()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f, groundMask))
        {
            Vector3 dir = hit.point - transform.position;
            dir.y = 0f;
            lastMouseDirection = dir.normalized;
        }
    }

    private void StartAttackSequence()
    {
        if (currentAttackCoroutine != null)
            StopCoroutine(currentAttackCoroutine);

        currentAttackCoroutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isPerformingAttack = true;

        do
        {
            attackQueued = false;

            int currentHit = (comboStep % 2) + 1;
            animator.SetTrigger($"Hit {currentHit}");
            comboStep = (comboStep + 1) % 2;

            // Applique uniquement la rotation si souris utilisée
            if (!usingGamepad)
            {
                Quaternion targetRot = Quaternion.LookRotation(lastMouseDirection);
                rb.MoveRotation(targetRot);
            }

            yield return new WaitForFixedUpdate();

            float endDash = Time.time + attackDashTime;
            while (Time.time < endDash)
            {
                rb.MovePosition(rb.position + transform.forward * attackDashSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }

            if (attackVFXPrefabs[currentHit - 1] && vfxSpawnPoint)
            {
                yield return new WaitForSeconds(0.1f);
                Instantiate(attackVFXPrefabs[currentHit - 1], vfxSpawnPoint.position, Quaternion.LookRotation(transform.forward));
            }

            if (attackSFX != null)
            {
                audioSource.PlayOneShot(attackSFX);
            }

            CheckForEnemiesHit();

            float elapsed = 0f;
            while (elapsed < attackAnimDuration && !attackQueued)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

        } while (attackQueued);

        isPerformingAttack = false;
    }

    private void CheckForEnemiesHit()
    {
        Vector3 center = transform.position + transform.forward * 2f;
        Collider[] hits = Physics.OverlapSphere(center, hitRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            var enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
                continue;
            }

            var destructible = hit.GetComponentInParent<Destructible>();
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

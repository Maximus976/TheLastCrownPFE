using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Santé")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    private int dieType;

    [Header("UI")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private MenuMort menuMort;
    [SerializeField] private GameObject hudUI;
    [SerializeField] private Image damageOverlay;

    [Header("Overlay personnalisé")]
    public Color overlayColor = Color.white;

    [Header("Effets caméra")]
    [SerializeField] private Volume globalVolume;
    private Vignette vignette;

    [Header("Effet vignette / overlay")]
    [SerializeField] private float fadeOutDelay = 5f;
    [SerializeField] private float fadeOutSpeed = 0.5f;

    [Header("Dependencies")]
    [SerializeField] private Animator animator;
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnDeath;

    private float timeSinceLastDamage = 0f;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        if (globalVolume != null && globalVolume.profile.TryGet(out Vignette v))
        {
            vignette = v;
        }

        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0f);
        }

        StartCoroutine(OverlayPulseEffect());
        StartCoroutine(VignetteFadeLogic());
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"[HEALTH] {gameObject.name} reçoit {amount} points de dégâts.");

        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        timeSinceLastDamage = 0f;

        UpdateHealthBar();

        if (CinemachineShake.instance != null)
            CinemachineShake.instance.ShakeCamera();

        if (currentHealth <= 0)
            Die();
    }

    public void RestoreHealth(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }
    public void ResetDeathState()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Réactive les scripts désactivés à la mort
        foreach (var script in scriptsToDisableOnDeath)
        {
            if (script != null)
                script.enabled = true;
        }

        // Réactive le HUD
        if (hudUI != null)
            hudUI.SetActive(true);

        // Reset de l'animator
        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.SetInteger("DieType", 0);
            animator.Play("Idle"); // Remplace "Idle" par ton animation par défaut de base
        }

        Debug.Log("[Health] État de mort réinitialisé.");
    }

    private IEnumerator VignetteFadeLogic()
    {
        while (true)
        {
            timeSinceLastDamage += Time.deltaTime;
            float hpPercent = (float)currentHealth / maxHealth;

            float targetIntensity = 0f;

            if (hpPercent <= 0.5f)
            {
                float severity = Mathf.InverseLerp(0.5f, 0f, hpPercent);
                targetIntensity = Mathf.Lerp(0f, 0.4f, severity);
            }

            if (timeSinceLastDamage >= fadeOutDelay)
            {
                float current = vignette.intensity.value;
                targetIntensity = Mathf.MoveTowards(current, 0f, Time.deltaTime * fadeOutSpeed);
            }

            if (vignette != null)
            {
                vignette.intensity.value = targetIntensity;
                vignette.smoothness.value = 1f;
            }

            yield return null;
        }
    }

    private IEnumerator OverlayPulseEffect()
    {
        while (true)
        {
            float hpPercent = (float)currentHealth / maxHealth;

            float baseAlpha = hpPercent <= 0.5f ? Mathf.Lerp(0f, 0.6f, 1f - hpPercent * 2f) : 0f;
            float pulseSpeed = Mathf.Lerp(1f, 5f, 1f - hpPercent * 2f);

            float t = 0f;
            while (t < Mathf.PI * 2f)
            {
                t += Time.deltaTime * pulseSpeed;
                float alpha = baseAlpha * (0.5f + Mathf.Sin(t) * 0.5f);

                if (timeSinceLastDamage >= fadeOutDelay)
                    alpha = Mathf.MoveTowards(damageOverlay.color.a, 0f, Time.deltaTime * fadeOutSpeed);

                if (damageOverlay != null)
                    damageOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, alpha);

                yield return null;
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        dieType = Random.Range(1, 5);

        if (animator != null)
        {
            animator.SetInteger("DieType", dieType);
            animator.SetTrigger("Die");
        }

        if (hudUI != null)
            hudUI.SetActive(false);

        foreach (var script in scriptsToDisableOnDeath)
        {
            if (script != null)
                script.enabled = false;
        }

        StartCoroutine(DelayedGameOver());
    }

    private IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(2f);

        // ✅ Respawn si un checkpoint est défini
        Chekpoint.Instance.RespawnPlayer(gameObject);

        // ✅ Réinitialiser la vie et les états
        currentHealth = maxHealth;
        isDead = false;

        if (hudUI != null)
            hudUI.SetActive(true);

        foreach (var script in scriptsToDisableOnDeath)
        {
            if (script != null)
                script.enabled = true;
        }

        animator.SetTrigger("Respawn");
        UpdateHealthBar();
    }
}
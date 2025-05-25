using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoosIntro : MonoBehaviour
{
    public CanvasGroup bossNameGroup;
    public TextMeshProUGUI bossNameText;
    public GameObject bossHealthBar;
    public CanvasGroup healthBarGroup;

    public float fadeDuration = 1f;
    public float nameDisplayTime = 2f;

    public GameObject playerObject; // ?? à assigner dans l’inspecteur

    void Start()
    {
        bossNameGroup.alpha = 0f;
        healthBarGroup.alpha = 0f;
        bossHealthBar.SetActive(false);
    }

    public void PlayIntro(string bossName)
    {
        bossNameText.text = bossName;
        StartCoroutine(ShowBossIntro());
    }

    private IEnumerator ShowBossIntro()
    {
        // ?? Désactiver le script de mouvement
        if (playerObject != null)
        {
            var movementScript = playerObject.GetComponent<PlayerMovement>();
            if (movementScript != null) movementScript.enabled = false;
        }

        // Fade in du nom
        yield return StartCoroutine(FadeCanvasGroup(bossNameGroup, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(nameDisplayTime);
        yield return StartCoroutine(FadeCanvasGroup(bossNameGroup, 1f, 0f, fadeDuration));

        // Afficher barre de vie
        bossHealthBar.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(healthBarGroup, 0f, 1f, fadeDuration));

        // ? Réactiver le mouvement
        if (playerObject != null)
        {
            var movementScript = playerObject.GetComponent<PlayerMovement>();
            if (movementScript != null) movementScript.enabled = true;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        group.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        group.alpha = to;
    }
}
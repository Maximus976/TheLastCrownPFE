using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintZone : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform floatingGroup; // Un GameObject parent du texte + images
    public float fadeDuration = 1f;
    public float floatAmplitude = 10f;
    public float floatSpeed = 2f;

    private Coroutine currentFade;
    private Coroutine floatRoutine;
    private Vector2 initialPos;

    private void Start()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (floatingGroup != null)
            initialPos = floatingGroup.anchoredPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeCanvasGroup(0f, 1f)); // Affiche complètement

            if (floatRoutine == null && floatingGroup != null)
                floatRoutine = StartCoroutine(Floating());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeCanvasGroup(canvasGroup.alpha, 0f));

            if (floatRoutine != null)
            {
                StopCoroutine(floatRoutine);
                floatRoutine = null;
                floatingGroup.anchoredPosition = initialPos; // Reset position
            }
        }
    }

    private IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }

    private IEnumerator Floating()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime * floatSpeed;
            float offsetY = Mathf.Sin(timer) * floatAmplitude;
            floatingGroup.anchoredPosition = initialPos + new Vector2(0f, offsetY);
            yield return null;
        }
    }
}
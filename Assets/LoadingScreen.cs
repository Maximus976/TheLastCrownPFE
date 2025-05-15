using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public CanvasGroup canvasGroup; // � assigner dans l'inspecteur
    public float fadeDuration = 1f;

    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        float startAlpha = canvasGroup.alpha;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false); // d�sactive le canvas apr�s le fade
    }
}
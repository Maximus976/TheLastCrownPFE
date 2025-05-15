using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Image fadeImage; // Image noire couvrant tout l'écran
    public float fadeDuration = 1.0f; // Durée du fade-out

    private void Start()
    {
        if (fadeImage != null)
        {
            // Assurez-vous que l'image est entièrement transparente au démarrage
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void TriggerFadeOut()
    {
        if (fadeImage != null)
        {
            StartCoroutine(fadeOut());
        }
    }

    private IEnumerator fadeOut()
    {
        if (fadeImage != null)
        {
            // Fade out (augmentation de l'alpha de 0 à 1 avec une courbe d'easing)
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                // Utilisation d'une courbe quadratique pour accélérer l'opacité
                float progress = t / fadeDuration;
                float alpha = Mathf.Pow(progress, 2); // Plus rapide au début
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            // Assurez-vous que l'image est complètement opaque
            fadeImage.color = new Color(0, 0, 0, 1);
        }
    }
}
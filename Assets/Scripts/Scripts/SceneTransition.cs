using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage; // Image noire couvrant tout l'écran
    public float fadeDuration = 1.0f; // Durée de la transition
    public string nextSceneName; // Nom de la scène suivante à charger

    private void Start()
    {
        if (fadeImage != null)
        {
            // Assurez-vous que l'image est complètement transparente au démarrage
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
        }
    }

    public void OnPlayButtonClicked()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float alpha = 1 - (t / fadeDuration); // Diminution de l'opacité
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 0); // Complètement transparent
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        if (fadeImage != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float alpha = t / fadeDuration; // Augmentation de l'opacité
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1); // Complètement opaque
        }

        // Une fois le fade-out terminé, charger la scène suivante
        SceneManager.LoadScene(nextSceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage; // Image noire couvrant tout l'�cran
    public float fadeDuration = 1.0f; // Dur�e de la transition
    public string nextSceneName; // Nom de la sc�ne suivante � charger

    private void Start()
    {
        if (fadeImage != null)
        {
            // Assurez-vous que l'image est compl�tement transparente au d�marrage
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
                float alpha = 1 - (t / fadeDuration); // Diminution de l'opacit�
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 0); // Compl�tement transparent
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        if (fadeImage != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float alpha = t / fadeDuration; // Augmentation de l'opacit�
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1); // Compl�tement opaque
        }

        // Une fois le fade-out termin�, charger la sc�ne suivante
        SceneManager.LoadScene(nextSceneName);
    }
}

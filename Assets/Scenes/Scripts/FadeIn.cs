using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class FadeIn : MonoBehaviour
{
    public Image fadeImage; // Image noire couvrant tout l'�cran
    public float fadeDuration = 1.0f; // Dur�e du fade-in

    private void Start()
    {
        if (fadeImage != null)
        {
            // Assurez-vous que l'image est enti�rement opaque au d�marrage (pour le fade-in)
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(fadeIn());
        }
    }

    private IEnumerator fadeIn()
    {
        if (fadeImage != null)
        {
            // D�lai avant le d�but du fondu
            yield return new WaitForSeconds(2f);

            // Fade in (diminution de l'alpha de 1 � 0 avec une courbe d'easing)
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float progress = t / fadeDuration;
                float alpha = 1 - Mathf.Pow(progress, 2);
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            // S'assurer que l'image est bien compl�tement transparente
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false); // Facultatif : d�sactiver l'image apr�s le fade
        }
    }
}

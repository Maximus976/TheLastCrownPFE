using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    public static SceneFade instance;

    public Image fadeImage;
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage != null)
        {
            // Optionnel : tu peux enlever le WaitForSeconds si tu veux pas attendre au début
            // yield return new WaitForSeconds(2f);

            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float progress = t / fadeDuration;
                float alpha = 1 - Mathf.Pow(progress, 2);
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }
    }

    public void FadeOutAndReloadScene()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeOutAndReload());
    }

    private IEnumerator FadeOutAndReload()
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float progress = t / fadeDuration;
            float alpha = Mathf.Pow(progress, 2);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

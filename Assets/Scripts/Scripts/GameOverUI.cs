using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public CanvasGroup gameOverPanel;
    public TextMeshProUGUI mortText;
    public float fadeDuration = 1f;
    public float moveDuration = 2f;
    public Vector3 moveOffset = new Vector3(0, 100f, 0);

    public GameObject[] buttons;
    public float delayBetweenButtons = 0.5f;

    private Vector3 initialTextPosition;

    void Awake()
    {
        gameOverPanel.alpha = 0f;
        gameOverPanel.blocksRaycasts = false;

        // Tu peux laisser cette ligne ou la supprimer si le texte est défini ailleurs :
        // mortText.text = "Vous êtes mort";

        initialTextPosition = mortText.rectTransform.anchoredPosition;

        foreach (var btn in buttons)
            btn.SetActive(false);
    }
    public void OnRetryPressed()
    {
        if (SceneFade.instance != null)
            SceneFade.instance.FadeOutAndReloadScene();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator GameOverSequence()
    {
        gameOverPanel.blocksRaycasts = true;

        // Fade-in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            gameOverPanel.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        gameOverPanel.alpha = 1f;

        // Texte qui monte
        t = 0;
        Vector3 startPos = initialTextPosition;
        Vector3 targetPos = startPos + moveOffset;

        while (t < moveDuration)
        {
            t += Time.unscaledDeltaTime;
            mortText.rectTransform.anchoredPosition = Vector3.Lerp(startPos, targetPos, t / moveDuration);
            yield return null;
        }
        mortText.rectTransform.anchoredPosition = targetPos;

        // Boutons qui apparaissent
        foreach (var btn in buttons)
        {
            btn.SetActive(true);
            yield return new WaitForSecondsRealtime(delayBetweenButtons);
        }
    }
}
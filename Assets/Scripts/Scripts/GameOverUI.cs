using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public CanvasGroup gameOverPanel;
    public TextMeshProUGUI mortText;
    public float fadeDuration = 1f;
    public float moveDuration = 2f;
    public Vector3 moveOffset = new Vector3(0, 100f, 0); // vers le haut

    public GameObject[] buttons;
    public float delayBetweenButtons = 0.5f;

    private Vector3 initialTextPosition;

    void Awake()
    {
        gameOverPanel.alpha = 0f;
        gameOverPanel.blocksRaycasts = false;

        // Juste pour tester
        mortText.text = "Vous êtes mort";

        initialTextPosition = mortText.rectTransform.anchoredPosition;

        foreach (var btn in buttons)
            btn.SetActive(false);
    }

    // Rendre public pour l'appeler depuis MenuMort
    public IEnumerator GameOverSequence()
    {
        gameOverPanel.blocksRaycasts = true;

        // Fade-in du panel
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // timeScale=0 donc unscaled
            gameOverPanel.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        gameOverPanel.alpha = 1f;

        // Déplacement du texte
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

        // Apparition progressive des boutons
        foreach (var btn in buttons)
        {
            btn.SetActive(true);
            yield return new WaitForSecondsRealtime(delayBetweenButtons);
        }
    }
}
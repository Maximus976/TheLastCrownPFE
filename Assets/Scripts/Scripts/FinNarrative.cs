using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FinNarrative : MonoBehaviour
{
    [System.Serializable]
    public class TextSequence
    {
        [TextArea] public string[] lines;
        public float duration = 8f;
    }

    public List<TextSequence> sequences = new List<TextSequence>();

    public GameObject textPrefab;
    public Transform textContainer;
    public GameObject creditsPanel;
    public GameObject finText;
    public CanvasGroup panelFader;

    public float fadeDuration = 2f;
    public string mainMenuSceneName = "Menu";

    private void Start()
    {
        creditsPanel.SetActive(false);
        finText.SetActive(false);
        panelFader.alpha = 0f;
        panelFader.gameObject.SetActive(true); // S'assurer qu�il est actif
        Time.timeScale = 1f;
    }

    // Appel public, avec d�lai configurable
    public void StartFinSequence(float delay = 2f)
    {
        StartCoroutine(DelayedStartFin(delay));
    }

    private IEnumerator DelayedStartFin(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // Stop jeu
        Time.timeScale = 0f;

        // �tape 1 : Fade vers noir
        yield return StartCoroutine(FadeCanvasGroup(panelFader, 0f, 1f, fadeDuration));
        yield return StartRealtime(1f); // Pause courte une fois noir

        // �tape 2 : Affichage des s�quences de texte
        foreach (var sequence in sequences)
        {
            // Supprime les anciens textes
            foreach (Transform child in textContainer)
                Destroy(child.gameObject);

            // Cr�e les nouveaux textes
            foreach (string line in sequence.lines)
            {
                GameObject textGO = Instantiate(textPrefab, textContainer);
                textGO.SetActive(true);
                textGO.GetComponent<TextMeshProUGUI>().text = line;
            }

            yield return StartRealtime(sequence.duration);
        }

        // Nettoyer les textes
        foreach (Transform child in textContainer)
            Destroy(child.gameObject);

        // �tape 3 : Cr�dits
        creditsPanel.SetActive(true);
        yield return StartRealtime(8f);
        creditsPanel.SetActive(false);

        // �tape 4 : "Fin"
        finText.SetActive(true);
        yield return StartRealtime(4f);

        // �tape 5 : Retour au menu
        yield return StartRealtime(2f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        group.alpha = from;
        group.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        group.alpha = to;
    }

    private IEnumerator StartRealtime(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
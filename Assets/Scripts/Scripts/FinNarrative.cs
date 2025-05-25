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
        public string position = "Middle"; // "Top", "Middle", "Bottom"
    }

    public List<TextSequence> sequences = new List<TextSequence>();

    public GameObject textPrefab;
    public Transform topContainer;
    public Transform middleContainer;
    public Transform bottomContainer;

    public GameObject creditsPanel;
    public GameObject finText;
    public CanvasGroup panelFader;

    public string playerTag = "Player";
    public float fadeDuration = 2f;
    public string mainMenuSceneName = "Menu";

    private MonoBehaviour playerMovementScript;

    void Start()
    {
        creditsPanel.SetActive(false);
        finText.SetActive(false);
        panelFader.alpha = 0f;

        // Trouve et désactive le script de mouvement du joueur
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerMovementScript = player.GetComponent<MonoBehaviour>(); // Remplace par le vrai nom de ton script si nécessaire
        }
    }

    public void StartFinSequence()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false; // désactive le contrôle du joueur
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        foreach (var sequence in sequences)
        {
            // Efface les anciens textes
            ClearAllContainers();

            Transform targetContainer = GetTargetContainer(sequence.position);
            if (targetContainer == null) targetContainer = middleContainer;

            foreach (string line in sequence.lines)
            {
                GameObject textGO = Instantiate(textPrefab, targetContainer);
                textGO.SetActive(true);
                textGO.GetComponent<TextMeshProUGUI>().text = line;
            }

            yield return new WaitForSeconds(sequence.duration);
        }

        ClearAllContainers();

        creditsPanel.SetActive(true);
        yield return new WaitForSeconds(8f);

        finText.SetActive(true);
        yield return new WaitForSeconds(4f);

        yield return StartCoroutine(FadeCanvasGroup(panelFader, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void ClearAllContainers()
    {
        foreach (Transform child in topContainer) Destroy(child.gameObject);
        foreach (Transform child in middleContainer) Destroy(child.gameObject);
        foreach (Transform child in bottomContainer) Destroy(child.gameObject);
    }

    private Transform GetTargetContainer(string position)
    {
        switch (position.ToLower())
        {
            case "top": return topContainer;
            case "bottom": return bottomContainer;
            case "middle":
            default: return middleContainer;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        group.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        group.alpha = to;
    }
}
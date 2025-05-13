using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChoiceManager : MonoBehaviour
{
    public TMP_Text narrativeText; // Texte de l'énigme en TMP
    public Button lancelotButton;
    public Button bedivereButton;
    public Button mordredButton;

    public AudioSource voiceOver;
    public AudioClip lancelotAudioClip;
    public AudioClip bedivereAudioClip;
    public AudioClip mordredAudioClip;

    private void Start()
    {
        narrativeText.text = "Ils m'ont juré loyauté, mais qui d'entre eux a tenu parole ?\nTrois noms. Trois vérités. Un seul cœur fidèle.";

        lancelotButton.onClick.AddListener(() => CheckChoice("Lancelot"));
        bedivereButton.onClick.AddListener(() => CheckChoice("Bédivère"));
        mordredButton.onClick.AddListener(() => CheckChoice("Mordred"));
    }

    private void CheckChoice(string choice)
    {
        switch (choice)
        {
            case "Lancelot":
                narrativeText.text = "Lancelot... L'amour a guidé ses actes, mais l'amour a brisé la table.";
                voiceOver.PlayOneShot(lancelotAudioClip);
                StartCoroutine(TransitionScene("SceneLancelot"));
                break;

            case "Bédivère":
                narrativeText.text = "Bédivère... Celui qui doutait, mais n'a jamais trahi. Son cœur était pur.";
                voiceOver.PlayOneShot(bedivereAudioClip);
                StartCoroutine(TransitionScene("SceneBédivère"));
                break;

            case "Mordred":
                narrativeText.text = "Mordred... Le sang du traître souille encore le sol de Camlann.";
                voiceOver.PlayOneShot(mordredAudioClip);
                StartCoroutine(TransitionScene("SceneMordred"));
                break;
        }
    }

    private IEnumerator TransitionScene(string sceneName)
    {
        yield return new WaitForSeconds(3f); // Laisser le temps au texte ou à la voix de finir
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
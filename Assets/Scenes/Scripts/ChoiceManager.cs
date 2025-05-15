using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChoiceManager : MonoBehaviour
{
    public TMP_Text narrativeText; // Texte de l'�nigme en TMP
    public Button lancelotButton;
    public Button bedivereButton;
    public Button mordredButton;

    public AudioSource voiceOver;
    public AudioClip lancelotAudioClip;
    public AudioClip bedivereAudioClip;
    public AudioClip mordredAudioClip;

    private void Start()
    {
        narrativeText.text = "Ils m'ont jur� loyaut�, mais qui d'entre eux a tenu parole ?\nTrois noms. Trois v�rit�s. Un seul c�ur fid�le.";

        lancelotButton.onClick.AddListener(() => CheckChoice("Lancelot"));
        bedivereButton.onClick.AddListener(() => CheckChoice("B�div�re"));
        mordredButton.onClick.AddListener(() => CheckChoice("Mordred"));
    }

    private void CheckChoice(string choice)
    {
        switch (choice)
        {
            case "Lancelot":
                narrativeText.text = "Lancelot... L'amour a guid� ses actes, mais l'amour a bris� la table.";
                voiceOver.PlayOneShot(lancelotAudioClip);
                StartCoroutine(TransitionScene("SceneLancelot"));
                break;

            case "B�div�re":
                narrativeText.text = "B�div�re... Celui qui doutait, mais n'a jamais trahi. Son c�ur �tait pur.";
                voiceOver.PlayOneShot(bedivereAudioClip);
                StartCoroutine(TransitionScene("SceneB�div�re"));
                break;

            case "Mordred":
                narrativeText.text = "Mordred... Le sang du tra�tre souille encore le sol de Camlann.";
                voiceOver.PlayOneShot(mordredAudioClip);
                StartCoroutine(TransitionScene("SceneMordred"));
                break;
        }
    }

    private IEnumerator TransitionScene(string sceneName)
    {
        yield return new WaitForSeconds(3f); // Laisser le temps au texte ou � la voix de finir
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
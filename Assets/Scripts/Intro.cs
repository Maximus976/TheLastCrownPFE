using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TextPair
{
    [TextArea]
    public string line1;
    [TextArea]
    public string line2;
    public float delayBeforeLine2 = 2f; // Délai personnalisé entre ligne 1 et ligne 2
}

public class Intro : MonoBehaviour
{
    [Header("UI Settings")]
    public TMP_Text line1Text;
    public TMP_Text line2Text;

    [Header("Text Pairs")]
    public List<TextPair> textPairs = new List<TextPair>();

    [Header("Timing")]
    public float initialDelay = 2f;
    public float fadeDuration = 1f;

    [Header("Music Settings")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;
    public float targetMusicVolume = 1f;
    public float musicFadeDuration = 2f;

    [Header("Voice Over")]
    public List<AudioClip> voiceClips = new List<AudioClip>();
    public float voiceFadeOutDuration = 0.5f;

    [Header("Scene Settings")]
    public string nextSceneName = "Scene2";

    private AudioSource voiceSource;

    void Awake()
    {
        textPairs = new List<TextPair>
        {
            new TextPair {
                line1 = "Écoute...\nCar ces eaux portent encore l’écho d’une histoire que peu connaissent.",
                line2 = "Une histoire que j’ai vécue, la sienne,\n celle d'Arthur.",
                delayBeforeLine2 = 3.6f
            },
            new TextPair {
                line1 = "Il n’était pas prince.\nNi noble, ni conquérant. Il n’était qu’un garçon, le coeur pur,\nle regard tourné vers les cimes des arbres.",
                line2 = "Un Bûcheron, façonné par la nature, et pourtant, le destin l'avait déjà choisi.",
                delayBeforeLine2 = 6f
            },
            new TextPair {
                line1 = "Merlin l’avait trouvé enfant.\nIl l’avait élevé, guidé, et préparé...",
                line2 = "Mais rien ne pouvait le préparer à ce jour.\nCelui où l’Ordre a brûlé son village, anéanti ses repères, et détruit sa paix.",
                delayBeforeLine2 = 3.5f
            },
            new TextPair {
                line1 = "C’est alors que le voile est tombé. Il a appris son nom véritable,",
                line2 = "\nle poids de son sang : héritier légitime du trône de Bretagne. Dernier espoir d'un royaume brisé...",
                delayBeforeLine2 = 1.8f
            },
            new TextPair {
                line1 = "Pour accomplir sa destinée,\nil devra traverser les ruines de l’ancien monde, se relever après chaque chute,",
                line2 = "et affronter Mordred... fruit d'un serment brisé.",
                delayBeforeLine2 = 5f
            },
            new TextPair {
                line1 = "Je suis la Dame du Lac.\nSon arme, sa mémoire.",
                line2 = "Et aujourd’hui, je vous raconte ce que j’ai vu,\navant que vous ne le découvriez par vous même.",
                delayBeforeLine2 = 3f
            }
        };
    }

    void Start()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = 0;
            musicSource.Play();
            StartCoroutine(FadeMusicIn());
        }

        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;

        line1Text.text = "";
        line2Text.text = "";
        StartCoroutine(DisplayIntroPairs());
    }

    IEnumerator FadeMusicIn()
    {
        float elapsed = 0f;
        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetMusicVolume, elapsed / musicFadeDuration);
            yield return null;
        }
        musicSource.volume = targetMusicVolume;
    }

    IEnumerator DisplayIntroPairs()
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < textPairs.Count; i++)
        {
            TextPair pair = textPairs[i];

            line1Text.text = pair.line1;
            line2Text.text = pair.line2;

            SetTextAlpha(line1Text, 0f);
            SetTextAlpha(line2Text, 0f);

            AudioClip clip = (i < voiceClips.Count) ? voiceClips[i] : null;
            float clipLength = (clip != null) ? clip.length : 8f;

            if (clip != null)
            {
                voiceSource.clip = clip;
                voiceSource.volume = 1f;
                voiceSource.Play();
            }

            yield return StartCoroutine(FadeText(line1Text, 0f, 1f, fadeDuration));
            yield return new WaitForSeconds(pair.delayBeforeLine2);
            yield return StartCoroutine(FadeText(line2Text, 0f, 1f, fadeDuration));

            float remainingTime = clipLength - pair.delayBeforeLine2 - voiceFadeOutDuration - fadeDuration;
            if (remainingTime > 0)
                yield return new WaitForSeconds(remainingTime);

            if (clip != null)
                yield return StartCoroutine(FadeOutVoice());

            yield return StartCoroutine(FadePair(line1Text, line2Text, 1f, 0f, fadeDuration));
        }

        if (musicSource != null)
            musicSource.Stop();

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeText(TMP_Text textComponent, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetTextAlpha(textComponent, alpha);
            yield return null;
        }
        SetTextAlpha(textComponent, endAlpha);
    }

    IEnumerator FadePair(TMP_Text t1, TMP_Text t2, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetTextAlpha(t1, alpha);
            SetTextAlpha(t2, alpha);
            yield return null;
        }
        SetTextAlpha(t1, endAlpha);
        SetTextAlpha(t2, endAlpha);
    }

    IEnumerator FadeOutVoice()
    {
        float startVolume = voiceSource.volume;
        float elapsed = 0f;
        while (elapsed < voiceFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            voiceSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / voiceFadeOutDuration);
            yield return null;
        }
        voiceSource.Stop();
        voiceSource.volume = 1f;
    }

    void SetTextAlpha(TMP_Text textComponent, float alpha)
    {
        Color c = textComponent.color;
        c.a = alpha;
        textComponent.color = c;
    }
}

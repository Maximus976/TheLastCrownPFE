using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TextPair
{
    [TextArea] public string line1;
    [TextArea] public string line2;
    public float delayBeforeLine2 = 2f;
}

public class Intro : MonoBehaviour
{
    [Header("UI Settings")]
    public TMP_Text line1Text;
    public TMP_Text line2Text;
    public Image imageDisplay;
    public Image fadeImage;

    [Header("Text Pairs")]
    public List<TextPair> textPairs = new List<TextPair>();

    [Header("Images")]
    public List<Sprite> images = new List<Sprite>();

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
                line1 = "\u00c9coute...\nCar ces eaux portent encore l\u2019\u00e9cho d\u2019une histoire que peu connaissent.",
                line2 = "Une histoire que j\u2019ai v\u00e9cue, la sienne,\n celle d'Arthur.",
                delayBeforeLine2 = 3.6f
            },
            new TextPair {
                line1 = "Il n\u2019\u00e9tait pas prince.\nNi noble, ni conqu\u00e9rant. Il n\u2019\u00e9tait qu\u2019un gar\u00e7on, le coeur pur,\nle regard tourn\u00e9 vers les cimes des arbres.",
                line2 = "Un B\u00fbcheron, fa\u00e7onn\u00e9 par la nature, et pourtant, le destin l'avait d\u00e9j\u00e0 choisi.",
                delayBeforeLine2 = 6f
            },
            new TextPair {
                line1 = "Merlin l\u2019avait trouv\u00e9 enfant.\nIl l\u2019avait \u00e9lev\u00e9, guid\u00e9, et pr\u00e9par\u00e9...",
                line2 = "Mais rien ne pouvait le pr\u00e9parer \u00e0 ce jour.\nCelui o\u00f9 l\u2019Ordre a br\u00fbl\u00e9 son village, an\u00e9anti ses rep\u00e8res, et d\u00e9truit sa paix.",
                delayBeforeLine2 = 3.5f
            },
            new TextPair {
                line1 = "C\u2019est alors que le voile est tomb\u00e9. Il a appris son nom v\u00e9ritable,",
                line2 = "\nle poids de son sang : h\u00e9ritier l\u00e9gitime du tr\u00f4ne de Bretagne. Dernier espoir d'un royaume bris\u00e9...",
                delayBeforeLine2 = 1.8f
            },
            new TextPair {
                line1 = "Pour accomplir sa destin\u00e9e,\nil devra traverser les ruines de l\u2019ancien monde, se relever apr\u00e8s chaque chute,",
                line2 = "et affronter Mordred... fruit d'un serment bris\u00e9.",
                delayBeforeLine2 = 5f
            },
            new TextPair {
                line1 = "Je suis la Dame du Lac.\nSon arme, sa m\u00e9moire.",
                line2 = "Et aujourd\u2019hui, je vous raconte ce que j\u2019ai vu,\navant que vous ne le d\u00e9couvriez par vous m\u00eame.",
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

            if (i < images.Count && imageDisplay != null)
            {
                imageDisplay.sprite = images[i];
            }

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

            float minDisplayTime = 2f;
            float remainingTime = clipLength - pair.delayBeforeLine2 - voiceFadeOutDuration - fadeDuration;
            yield return new WaitForSeconds(Mathf.Max(minDisplayTime, remainingTime));

            if (clip != null)
                yield return StartCoroutine(FadeOutVoice());

            yield return StartCoroutine(FadePair(line1Text, line2Text, 1f, 0f, fadeDuration));
        }

        if (musicSource != null)
            musicSource.Stop();

        yield return StartCoroutine(FadeToBlack());
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

    IEnumerator FadeToBlack()
    {
        if (fadeImage == null)
            yield break;

        Color color = fadeImage.color;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    void SetTextAlpha(TMP_Text textComponent, float alpha)
    {
        Color c = textComponent.color;
        c.a = alpha;
        textComponent.color = c;
    }
}
using UnityEngine;
using UnityEngine.UI;             // Si tu utilises UI Text
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
}

public class Intro : MonoBehaviour
{
    [Header("UI Settings")]
    public Text line1Text;
    public Text line2Text;

    [Header("Text Pairs")]
    public List<TextPair> textPairs = new List<TextPair>();

    [Header("Timing")]
    public float initialDelay = 2f;
    public float fadeDuration = 1f;
    public float waitAfterFadeIn = 2f;

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
        textPairs = new List<TextPair>()
        {
            new TextPair {
                line1 = "Au cœur de Camelot, le destin d'un roi se dessine,",
                line2 = "les murmures du passé annoncent la renaissance d'une ère glorieuse."
            },
            new TextPair {
                line1 = "L'épée Excalibur, gardienne d'un pouvoir sacré,",
                line2 = "attend d'être brandie par celui qui sauvera le royaume."
            },
            new TextPair {
                line1 = "Dans les brumes mystiques, Merlin dévoile des prophéties oubliées,",
                line2 = "guidant les âmes vaillantes vers la lumière."
            },
            new TextPair {
                line1 = "Les chevaliers de la Table Ronde jurent fidélité à la justice,",
                line2 = "leur courage illuminant l'obscurité des temps troublés."
            },
            new TextPair {
                line1 = "Face aux puissances obscures qui menacent la sérénité de Camelot,",
                line2 = "l'esprit d'Arthur s'élève pour restaurer l'honneur et la vérité."
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

            // Lancement de la voix
            AudioClip clip = (i < voiceClips.Count) ? voiceClips[i] : null;
            if (clip != null)
            {
                voiceSource.clip = clip;
                voiceSource.volume = 1f;
                voiceSource.Play();
            }

            // Affiche la première phrase
            yield return StartCoroutine(FadeText(line1Text, 0f, 1f, fadeDuration));

            // Attend 4 secondes avant d'afficher la deuxième phrase
            yield return new WaitForSeconds(2f);

            // Affiche la deuxième phrase
            yield return StartCoroutine(FadeText(line2Text, 0f, 1f, fadeDuration));

            // Attend le reste du clip (8 - 4 - voiceFadeOut)
            float remainingTime = 8f - 4f - voiceFadeOutDuration;
            yield return new WaitForSeconds(remainingTime);

            // Fade out de la voix
            if (clip != null)
                yield return StartCoroutine(FadeOutVoice());

            // Disparition du texte
            yield return StartCoroutine(FadePair(line1Text, line2Text, 1f, 0f, fadeDuration));
        }

        if (musicSource != null)
            musicSource.Stop();

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeText(Text textComponent, float startAlpha, float endAlpha, float duration)
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

    IEnumerator FadePair(Text t1, Text t2, float startAlpha, float endAlpha, float duration)
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

    void SetTextAlpha(Text textComponent, float alpha)
    {
        Color c = textComponent.color;
        c.a = alpha;
        textComponent.color = c;
    }
}
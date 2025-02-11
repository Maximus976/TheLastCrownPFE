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
    [Tooltip("UI Text pour la premi�re ligne")]
    public Text line1Text;
    [Tooltip("UI Text pour la deuxi�me ligne")]
    public Text line2Text;

    [Header("Text Pairs")]
    [Tooltip("Liste des paires de textes � afficher")]
    public List<TextPair> textPairs = new List<TextPair>();

    [Header("Timing")]
    [Tooltip("D�lai avant de commencer � afficher la premi�re paire de texte")]
    public float initialDelay = 2f;
    [Tooltip("Dur�e du fade (in et out) pour chaque ligne, en secondes")]
    public float fadeDuration = 1f;
    [Tooltip("Temps pendant lequel la paire reste visible apr�s les fade in, en secondes")]
    public float waitAfterFadeIn = 2f;

    [Header("Music Settings")]
    [Tooltip("AudioSource d�di� � la musique de fond")]
    public AudioSource musicSource;
    [Tooltip("Clip audio de la musique d'intro")]
    public AudioClip backgroundMusic;
    [Tooltip("Volume cible de la musique une fois le fade in termin�")]
    public float targetMusicVolume = 1f;
    [Tooltip("Dur�e du fade in pour la musique")]
    public float musicFadeDuration = 2f;

    [Header("Scene Settings")]
    [Tooltip("Nom de la sc�ne � charger � la fin de l'intro")]
    public string nextSceneName = "Scene2";

    void Awake()
    {
        // Initialisation automatique des paires de textes
        textPairs = new List<TextPair>()
        {
            new TextPair { line1 = "Dans un monde oubli� par le temps,", line2 = "les t�n�bres murmurent d'anciens secrets." },
            new TextPair { line1 = "Au c�ur de l'ombre na�t une lueur fragile,", line2 = "promesse d'espoir dans l'obscurit�." },
            new TextPair { line1 = "Un h�ros, guid� par la lumi�re int�rieure,", line2 = "se dresse contre les forces oubli�es." },
            new TextPair { line1 = "Son p�riple l'entra�ne vers des terres mystiques,", line2 = "o� chaque pas r�veille des l�gendes endormies." },
            new TextPair { line1 = "Au fil du temps, la lumi�re triomphe,", line2 = "et un nouveau jour se l�ve sur le royaume." }
        };
    }

    void Start()
    {
        // D�marre la musique si le clip est assign�, avec fade in
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; // pour que la musique se r�p�te
            musicSource.volume = 0;  // Commencer avec le volume � 0
            musicSource.Play();
            StartCoroutine(FadeMusicIn());
        }

        // S'assurer que les textes sont vides au d�part
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
        // Attendre le d�lai initial avant de d�marrer l'animation de la premi�re paire
        yield return new WaitForSeconds(initialDelay);

        // Pour chaque paire dans la liste
        foreach (TextPair pair in textPairs)
        {
            // Assigne le contenu des textes
            line1Text.text = pair.line1;
            line2Text.text = pair.line2;

            // Initialise l'opacit� � 0 pour les deux lignes
            SetTextAlpha(line1Text, 0f);
            SetTextAlpha(line2Text, 0f);

            // Fade in de la premi�re ligne
            yield return StartCoroutine(FadeText(line1Text, 0f, 1f, fadeDuration));

            // Fade in de la deuxi�me ligne (apr�s que la premi�re soit compl�te)
            yield return StartCoroutine(FadeText(line2Text, 0f, 1f, fadeDuration));

            // Attend que le joueur lise la paire
            yield return new WaitForSeconds(waitAfterFadeIn);

            // Fade out simultan� des deux lignes
            yield return StartCoroutine(FadePair(line1Text, line2Text, 1f, 0f, fadeDuration));
        }

        // Optionnel : Arr�ter la musique si besoin
        if (musicSource != null)
            musicSource.Stop();

        // Une fois toutes les paires affich�es, changer de sc�ne
        SceneManager.LoadScene(nextSceneName);
    }

    // Coroutine pour faire appara�tre ou dispara�tre un seul texte progressivement
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

    // Coroutine pour faire le fade de deux textes simultan�ment
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

    // M�thode utilitaire pour d�finir l'opacit� (alpha) d'un UI Text
    void SetTextAlpha(Text textComponent, float alpha)
    {
        Color c = textComponent.color;
        c.a = alpha;
        textComponent.color = c;
    }
}
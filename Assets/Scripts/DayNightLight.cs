using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightLight : MonoBehaviour
{
    public Light sun;
    public float speed = 10f;

    public Light[] nightLights;        // Lumières de la nuit
    public GameObject[] firefliesList; // Liste des lucioles
    public GameObject insectSoundManagerPrefab;  // Préfabriqué pour gérer les sources audio

    public AudioClip[] insectSounds;   // Liste des sons d'insectes (nuit)
    public AudioClip[] daySounds;      // Liste des sons de la journée

    public float transitionSpeed = 2f;  // Vitesse de transition des lumières
    public float lightDelay = 0.5f;     // Délai entre chaque lumière qui s'allume
    public float fireflyDelay = 0.3f;   // Délai entre chaque apparition de lucioles
    public float soundFadeDuration = 2f;  // Durée de transition du son
    public float soundIntervalMin = 5f;  // Intervalle minimum entre les sons des insectes
    public float soundIntervalMax = 15f; // Intervalle maximum entre les sons des insectes
    public float maxSoundVolume = 0.1f;  // Volume maximal pour les sons

    private bool isNight = false;
    private float[] maxIntensities;

    private AudioSource[] insectAudioSources;  // Sources audio pour les insectes
    private AudioSource[] dayAudioSources;     // Sources audio pour la journée

    void Start()
    {
        if (sun == null)
            sun = GetComponent<Light>();

        // Sauvegarde des intensités maximales pour restaurer plus tard
        maxIntensities = new float[nightLights.Length];
        for (int i = 0; i < nightLights.Length; i++)
        {
            maxIntensities[i] = nightLights[i].intensity;
            nightLights[i].intensity = 0f; // Début avec lumières éteintes
        }

        // Désactiver les lucioles au début
        foreach (GameObject firefly in firefliesList)
        {
            firefly.SetActive(false);
        }

        // Initialiser les sources audio d'insectes
        insectAudioSources = new AudioSource[insectSounds.Length];
        for (int i = 0; i < insectSounds.Length; i++)
        {
            GameObject soundObject = Instantiate(insectSoundManagerPrefab, transform);
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = insectSounds[i];
            audioSource.loop = true;  // Pour jouer en boucle
            audioSource.volume = 0f;  // Commencer avec volume à 0
            insectAudioSources[i] = audioSource;
        }

        // Initialiser les sources audio de la journée
        dayAudioSources = new AudioSource[daySounds.Length];
        for (int i = 0; i < daySounds.Length; i++)
        {
            GameObject soundObject = Instantiate(insectSoundManagerPrefab, transform);
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = daySounds[i];
            audioSource.loop = true;  // Pour jouer en boucle
            audioSource.volume = 0f;  // Commencer avec volume à 0
            dayAudioSources[i] = audioSource;
        }
    }

    void Update()
    {
        // Rotation du soleil
        sun.transform.Rotate(Vector3.right * speed * Time.deltaTime);

        // Détection de la nuit/jour selon l'angle du soleil
        if (sun.transform.eulerAngles.x > 180 && !isNight)
        {
            isNight = true;
            StopAllCoroutines();
            StartCoroutine(ActivateNightElements(1f));  // Transition vers la nuit
        }
        else if (sun.transform.eulerAngles.x <= 180 && isNight)
        {
            isNight = false;
            StopAllCoroutines();
            StartCoroutine(ActivateNightElements(0f));  // Transition vers le jour
        }
    }

    IEnumerator ActivateNightElements(float targetFactor)
    {
        // Transition des lumières de nuit avec décalage
        for (int i = 0; i < nightLights.Length; i++)
        {
            StartCoroutine(SmoothLightIntensity(nightLights[i], maxIntensities[i] * targetFactor));
            yield return new WaitForSeconds(lightDelay);
        }

        // Transition des lucioles avec décalage
        for (int i = 0; i < firefliesList.Length; i++)
        {
            firefliesList[i].SetActive(targetFactor > 0.5f);
            yield return new WaitForSeconds(fireflyDelay);
        }

        // Activation du son des insectes et des sons de jour/nuit en boucle
        if (targetFactor > 0.5f) // Si c'est la nuit
        {
            StartCoroutine(FadeInSounds(insectAudioSources));
            StartCoroutine(FadeOutSounds(dayAudioSources));
        }
        else // Si c'est le jour
        {
            StartCoroutine(FadeInSounds(dayAudioSources));
            StartCoroutine(FadeOutSounds(insectAudioSources));
        }
    }
    IEnumerator SmoothLightIntensity(Light light, float targetIntensity)
    {
        float duration = 0f;
        float startIntensity = light.intensity;

        while (duration < 1f)
        {
            duration += Time.deltaTime * transitionSpeed;
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, duration);
            yield return null;
        }

        light.intensity = targetIntensity;  // Assurer la valeur finale exacte
    }

    IEnumerator FadeInSounds(AudioSource[] audioSources)
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].volume = 0f;
            if (!audioSources[i].isPlaying)
                audioSources[i].Play();  // Démarrer la lecture du son

            while (audioSources[i].volume < maxSoundVolume)
            {
                audioSources[i].volume += Time.deltaTime / soundFadeDuration;
                yield return null;
            }

            audioSources[i].volume = maxSoundVolume;  // Assurer que le volume atteint la valeur maximale
        }
    }
    IEnumerator FadeOutSounds(AudioSource[] audioSources)
    {
        // Fade-out pour chaque source audio
        for (int i = 0; i < audioSources.Length; i++)
        {
            // Fade-out pour diminuer le volume
            while (audioSources[i].volume > 0f)
            {
                audioSources[i].volume -= Time.deltaTime / soundFadeDuration;
                yield return null;
            }

            audioSources[i].Stop();  // Arrêter le son lorsque le volume atteint zéro
            audioSources[i].volume = 0f;  // Assurer que le volume final est à zéro
        }
    }

    IEnumerator PlayRandomInsectSounds()
    {
        while (isNight)
        {
            // Choisir un son aléatoire parmi la liste des sons
            int randomIndex = Random.Range(0, insectAudioSources.Length);
            insectAudioSources[randomIndex].PlayOneShot(insectAudioSources[randomIndex].clip);

            // Attendre un intervalle aléatoire avant de jouer le prochain son
            float randomInterval = Random.Range(soundIntervalMin, soundIntervalMax);
            yield return new WaitForSeconds(randomInterval);
        }
    }
}
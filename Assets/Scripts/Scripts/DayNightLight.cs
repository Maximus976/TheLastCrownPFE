using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightLight : MonoBehaviour
{
    public Light sun;
    public float speed = 10f;

    public Light[] nightLights;       
    public GameObject[] firefliesList; 
    public GameObject insectSoundManagerPrefab; 

    public AudioClip[] insectSounds;  
    public AudioClip[] daySounds;      

    public float transitionSpeed = 2f;  
    public float lightDelay = 0.5f;     
    public float fireflyDelay = 0.3f;   
    public float soundFadeDuration = 2f;  
    public float soundIntervalMin = 5f;  
    public float soundIntervalMax = 15f; 
    public float maxSoundVolume = 0.1f;  

    private bool isNight = false;
    private float[] maxIntensities;

    private AudioSource[] insectAudioSources;
    private AudioSource[] dayAudioSources;
    public Color dayFogColor = Color.gray;
    public Color nightFogColor = new Color(0.05f, 0.05f, 0.2f);
    public float fogTransitionSpeed = 1f;
   
    void Start()
    {
        if (sun == null)
            sun = GetComponent<Light>();

      
        maxIntensities = new float[nightLights.Length];
        for (int i = 0; i < nightLights.Length; i++)
        {
            maxIntensities[i] = nightLights[i].intensity;
            nightLights[i].intensity = 0f; 
        }

    
        foreach (GameObject firefly in firefliesList)
        {
            firefly.SetActive(false);
        }

        
        insectAudioSources = new AudioSource[insectSounds.Length];
        for (int i = 0; i < insectSounds.Length; i++)
        {
            GameObject soundObject = Instantiate(insectSoundManagerPrefab, transform);
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = insectSounds[i];
            audioSource.loop = true;  
            audioSource.volume = 0f;  
            insectAudioSources[i] = audioSource;
        }

       
        dayAudioSources = new AudioSource[daySounds.Length];
        for (int i = 0; i < daySounds.Length; i++)
        {
            GameObject soundObject = Instantiate(insectSoundManagerPrefab, transform);
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = daySounds[i];
            audioSource.loop = true;  
            audioSource.volume = 0f; 
            dayAudioSources[i] = audioSource;
        }
    }

    void Update()
    {
        
        sun.transform.Rotate(Vector3.right * speed * Time.deltaTime);

 
        if (sun.transform.eulerAngles.x > 180 && !isNight)
        {
            isNight = true;
            StopAllCoroutines();
            StartCoroutine(ActivateNightElements(1f));  
        }
        else if (sun.transform.eulerAngles.x <= 180 && isNight)
        {
            isNight = false;
            StopAllCoroutines();
            StartCoroutine(ActivateNightElements(0f));  
        }
    }

   IEnumerator ActivateNightElements(float targetFactor)
    {
        // Transition du fog en premier
        StartCoroutine(ChangeFogColor(targetFactor > 0.5f ? nightFogColor : dayFogColor));

        // Attendre un peu pour donner l'effet que le fog s'installe avant les autres changements
        yield return new WaitForSeconds(1f);

        // Transition des lumières de nuit
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

        // Gestion des sons
        if (targetFactor > 0.5f)
        {
            StartCoroutine(FadeInSounds(insectAudioSources));
            StartCoroutine(FadeOutSounds(dayAudioSources));
        }
        else
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

        light.intensity = targetIntensity;  
    }

    IEnumerator FadeInSounds(AudioSource[] audioSources)
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].volume = 0f;
            if (!audioSources[i].isPlaying)
                audioSources[i].Play(); 

            while (audioSources[i].volume < maxSoundVolume)
            {
                audioSources[i].volume += Time.deltaTime / soundFadeDuration;
                yield return null;
            }

            audioSources[i].volume = maxSoundVolume;  
        }
    }
    IEnumerator FadeOutSounds(AudioSource[] audioSources)
    {
       
        for (int i = 0; i < audioSources.Length; i++)
        {
            
            while (audioSources[i].volume > 0f)
            {
                audioSources[i].volume -= Time.deltaTime / soundFadeDuration;
                yield return null;
            }

            audioSources[i].Stop();  
            audioSources[i].volume = 0f; 
        }
    }

    IEnumerator PlayRandomInsectSounds()
    {
        while (isNight)
        {
          
            int randomIndex = Random.Range(0, insectAudioSources.Length);
            insectAudioSources[randomIndex].PlayOneShot(insectAudioSources[randomIndex].clip);

            
            float randomInterval = Random.Range(soundIntervalMin, soundIntervalMax);
            yield return new WaitForSeconds(randomInterval);
        }
    }
    IEnumerator ChangeFogColor(Color targetColor)
    {
        Color startColor = RenderSettings.fogColor;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * fogTransitionSpeed;
            RenderSettings.fogColor = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        RenderSettings.fogColor = targetColor;
    }
}
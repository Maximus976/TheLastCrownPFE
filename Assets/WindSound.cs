/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSound : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip windClip;
    [Range(0f, 1f)] public float volume = 0.5f;
    public bool playOnStart = true;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (windClip != null)
        {
            audioSource.clip = windClip;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.volume = volume;
        }
        else
        {
            Debug.LogWarning($"[WindSound] Aucun clip assigné sur {gameObject.name} !");
        }
    }

    void Start()
    {
        if (playOnStart && windClip != null)
        {
            audioSource.Play();
        }
    }

    public void PlayWind()
    {
        if (!audioSource.isPlaying && windClip != null)
            audioSource.Play();
    }

    public void StopWind()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}*/
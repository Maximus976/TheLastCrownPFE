using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CascadeAudio : MonoBehaviour
{
    public Transform joueur;
    public float distanceMax = 30f; // distance à partir de laquelle on n'entend plus rien
    public float distanceMin = 5f;  // distance où le son est au maximum

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void Update()
    {
        if (joueur == null) return;

        float distance = Vector3.Distance(transform.position, joueur.position);

        if (distance > distanceMax)
        {
            audioSource.volume = 0f;
        }
        else
        {
            float volume = 1f - Mathf.InverseLerp(distanceMin, distanceMax, distance);
            audioSource.volume = volume;
        }
    }
}
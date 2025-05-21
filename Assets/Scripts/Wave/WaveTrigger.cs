using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public WaveSpawner waveSpawner;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Le joueur est entré dans la zone. Déclenchement de la vague 1.");
            waveSpawner.StartFirstWave();
        }
    }
}

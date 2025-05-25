using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WaveTrigger : MonoBehaviour
{
    public WaveSpawner waveSpawner;
    public CinemachineVirtualCamera arenaCamera;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Le joueur est entré dans la zone. Déclenchement de la vague 1.");

            // Active la caméra d’arène
            if (arenaCamera != null)
                arenaCamera.Priority = 20;

            waveSpawner.StartFirstWave();
        }
    }
}

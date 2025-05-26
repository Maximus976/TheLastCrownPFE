using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public int torchIndex;
    public TorchManager manager;

    public GameObject flameEffect;
    public AudioSource fireWhooshSound;      // son au moment de l’allumage
    public AudioSource fireLoopSound;        // son en boucle
    public AudioSource fireOffSound;         // son à l’extinction

    [Header("Audio distance")]
    public Transform player;                 // assigner le joueur (ou sa caméra) ici
    public float audibleDistance = 15f;      // distance maximale d’audibilité manuelle (en plus de la spatialisation Unity)

    private bool isLit = false;
    private bool isLocked = false;
    private bool playerInRange = false;

    void Update()
    {
        // Interaction joueur
        if (playerInRange && !isLocked && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)))
        {
            manager.OnTorchInteraction(this);
        }

        // Ajustement du volume de la boucle en fonction de la distance
        if (fireLoopSound != null && player != null && fireLoopSound.isPlaying)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            float t = Mathf.Clamp01(1f - (distance / audibleDistance));
            fireLoopSound.volume = t;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    public void Light()
    {
        isLit = true;

        if (flameEffect != null)
            flameEffect.SetActive(true);

        if (fireWhooshSound != null)
            fireWhooshSound.Play();

        if (fireLoopSound != null)
        {
            fireLoopSound.volume = 1f; // volume max au départ
            fireLoopSound.Play();
        }
    }

    public void Extinguish()
    {
        isLit = false;
        isLocked = false;

        if (flameEffect != null)
            flameEffect.SetActive(false);

        if (fireLoopSound != null)
            fireLoopSound.Stop();

        if (fireOffSound != null)
            fireOffSound.Play();
    }

    public void Lock()
    {
        isLocked = true;
        if (!isLit) Light(); // allume si jamais ce n’est pas fait
    }

    public bool IsLit()
    {
        return isLit;
    }
}
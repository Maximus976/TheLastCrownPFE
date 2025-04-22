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

    private bool isLit = false;
    private bool isLocked = false;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && !isLocked && Input.GetKeyDown(KeyCode.E))
        {
            manager.OnTorchInteraction(this);
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

        if (fireWhooshSound) fireWhooshSound.Play();
        if (fireLoopSound) fireLoopSound.Play();
    }

    public void Extinguish()
    {
        isLit = false;
        isLocked = false;

        if (flameEffect != null)
            flameEffect.SetActive(false);

        if (fireLoopSound) fireLoopSound.Stop();
        if (fireOffSound) fireOffSound.Play();
    }

    public void Lock()
    {
        isLocked = true;
        if (!isLit) Light(); // au cas où
    }

    public bool IsLit()
    {
        return isLit;
    }
}
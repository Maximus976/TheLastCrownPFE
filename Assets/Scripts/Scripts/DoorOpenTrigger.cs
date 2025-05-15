using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
    public Animator porteAnimator; // À assigner dans l’inspecteur

    private bool aDejaOuvert = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Quelque chose est entré : " + other.name);

        if (other.CompareTag("Player") && !aDejaOuvert)
        {
            Debug.Log("C’est le joueur !");
            aDejaOuvert = true;
            porteAnimator.SetTrigger("Open 0");
        }
    }
}
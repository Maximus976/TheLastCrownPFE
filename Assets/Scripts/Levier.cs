using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levier : MonoBehaviour
{
    public Grille grille; // À assigner dans l’inspecteur
    private bool joueurDansZone = false;

    void Update()
    {
        if (joueurDansZone && Input.GetKeyDown(KeyCode.E))
        {
            grille.ActiverGrille();
            // Tu peux ici jouer une animation ou un son si tu veux
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = true;
            // Affiche une UI du type "Appuyez sur E pour activer"
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            // Cache l'UI d'interaction
        }
    }
}

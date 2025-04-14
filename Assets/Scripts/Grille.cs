using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grille : MonoBehaviour
{
    // Variable statique pour suivre la grille actuellement ouverte (s'il y en a une)
    public static Grille grilleOuverte = null;

    public Transform[] barreaux;           // Les 5 barreaux
    public Vector3[] positionsFermees;     // Positions locales fermées
    public Vector3[] positionsOuvertes;    // Positions locales ouvertes
    public float vitesse = 2f;             // Vitesse de mouvement

    private Vector3[] destinations;        // Destination actuelle de chaque barreau
    private bool enMouvement = false;
    private bool estOuverte = false;
    private Collider[] colliders;          // Colliders des barreaux

    void Start()
    {
        destinations = new Vector3[barreaux.Length];
        colliders = new Collider[barreaux.Length];

        for (int i = 0; i < barreaux.Length; i++)
        {
            barreaux[i].localPosition = positionsFermees[i];
            destinations[i] = positionsFermees[i];

            colliders[i] = barreaux[i].GetComponent<Collider>();
            if (colliders[i] != null)
                colliders[i].enabled = true;
        }

        estOuverte = false;
    }

    void Update()
    {
        if (enMouvement)
        {
            bool tousArrives = true;

            for (int i = 0; i < barreaux.Length; i++)
            {
                barreaux[i].localPosition = Vector3.MoveTowards(
                    barreaux[i].localPosition,
                    destinations[i],
                    vitesse * Time.deltaTime
                );

                if (Vector3.Distance(barreaux[i].localPosition, destinations[i]) > 0.01f)
                {
                    tousArrives = false;
                }
            }

            if (tousArrives)
            {
                enMouvement = false;

                // Si la grille est ouverte, désactiver les colliders
                if (estOuverte)
                {
                    foreach (Collider col in colliders)
                        col.enabled = false;
                }
            }
        }
    }

    // Fonction à appeler depuis un levier
    public void ActiverGrille()
    {
        if (enMouvement) return;

        if (estOuverte)
            FermerGrille();
        else
            OuvrirGrille();
    }

    public void OuvrirGrille()
    {
        // Fermer une autre grille si nécessaire
        if (grilleOuverte != null && grilleOuverte != this)
        {
            grilleOuverte.FermerGrille();
        }

        for (int i = 0; i < barreaux.Length; i++)
        {
            destinations[i] = positionsOuvertes[i];
        }

        enMouvement = true;
        estOuverte = true;
        grilleOuverte = this;
    }

    public void FermerGrille()
    {
        for (int i = 0; i < barreaux.Length; i++)
        {
            destinations[i] = positionsFermees[i];
        }

        enMouvement = true;
        estOuverte = false;

        // Réactiver les colliders
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        if (grilleOuverte == this)
        {
            grilleOuverte = null;
        }
    }
}

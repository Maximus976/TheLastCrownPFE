using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grille : MonoBehaviour
{
    public Transform[] barreaux;
    public Vector3[] positionsFermees;
    public Vector3[] positionsOuvertes;
    public float vitesse = 2f;

    private Vector3[] destinations;
    private bool enMouvement = false;
    private bool estOuverte = false;
    private Collider[] colliders;

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

                // Désactiver les colliders si ouverte
                if (estOuverte)
                {
                    foreach (Collider col in colliders)
                        col.enabled = false;
                }
                else
                {
                    foreach (Collider col in colliders)
                        col.enabled = true;
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
        for (int i = 0; i < barreaux.Length; i++)
        {
            destinations[i] = positionsOuvertes[i];
        }

        enMouvement = true;
        estOuverte = true;
    }

    public void FermerGrille()
    {
        for (int i = 0; i < barreaux.Length; i++)
        {
            destinations[i] = positionsFermees[i];
        }

        enMouvement = true;
        estOuverte = false;
    }
}
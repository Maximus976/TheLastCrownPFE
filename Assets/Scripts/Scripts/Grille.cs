using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grille : MonoBehaviour
{
    public Transform[] barreaux;
    public Vector3[] positionsFermees;
    public Vector3[] positionsOuvertes;
    public float vitesse = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clipOuverture;

    private Vector3[] destinations;
    private bool enMouvement = false;
    private bool estOuverte = false;
    private bool aDÈj‡Ouvert = false;
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
        aDÈj‡Ouvert = false;
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

                // DÈsactiver les colliders si grille ouverte
                foreach (Collider col in colliders)
                    col.enabled = !estOuverte;
            }
        }
    }

    // Appel depuis un levier ou un trigger
    public void ActiverGrille()
    {
        if (enMouvement) return;

        if (!estOuverte && !aDÈj‡Ouvert)
        {
            OuvrirGrille();
        }
        // Sinon, ne fait rien (empÍche la fermeture aprËs ouverture)
    }

    public void OuvrirGrille()
    {
        for (int i = 0; i < barreaux.Length; i++)
        {
            destinations[i] = positionsOuvertes[i];
        }

        enMouvement = true;
        estOuverte = true;
        aDÈj‡Ouvert = true;

        if (audioSource != null && clipOuverture != null)
        {
            audioSource.PlayOneShot(clipOuverture);
        }
    }
}
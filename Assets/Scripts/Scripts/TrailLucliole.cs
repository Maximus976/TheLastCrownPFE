using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailLucliole : MonoBehaviour
{
    [Header("Effet de luciole")]
    public GameObject fireflyVFXPrefab; // Le prefab de la luciole (Particle System)
    public float movementRange = 2f;    // Plage de mouvement al�atoire des lucioles
    public float lifetime = 3f;         // Dur�e de vie des lucioles avant de dispara�tre
    public float emissionRate = 0.5f;   // Taux d'�mission des lucioles par seconde

    private GameObject[] fireflies;  // Tableau pour stocker les lucioles actives

    private void Start()
    {
        // Cr�er et activer les lucioles d�s le d�part
        fireflies = new GameObject[10]; // Par exemple, 10 lucioles au d�part

        for (int i = 0; i < fireflies.Length; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-movementRange, movementRange),
                Random.Range(-movementRange, movementRange),
                0
            );

            fireflies[i] = Instantiate(fireflyVFXPrefab, spawnPosition, Quaternion.identity);
            fireflies[i].transform.SetParent(transform);  // Les lier au mur
        }
    }

    // Fonction pour d�sactiver toutes les lucioles
    public void DestroyFireflies()
    {
        foreach (GameObject firefly in fireflies)
        {
            if (firefly != null)
            {
                Destroy(firefly); // D�truire les lucioles � la destruction du mur
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurDestructible : MonoBehaviour
{
    public GameObject murEboulementPrefab;   // Le prefab du mur en ruine
    public float shakeDuration = 0.5f;       // Durée du shake
    public float shakeIntensity = 0.1f;      // Intensité du shake

    public void DetruireMur()
    {
        StartCoroutine(ShakeThenDestroy());
    }

    private IEnumerator ShakeThenDestroy()
    {
        Vector3 originalPosition = transform.position;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity)
            );

            transform.position = originalPosition + shakeOffset;
            yield return null;
        }

        transform.position = originalPosition; // Remet en place le mur

        if (murEboulementPrefab != null)
        {
            Instantiate(murEboulementPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("?? Aucun prefab d’éboulement assigné !");
        }

        Destroy(gameObject); // Détruit le mur original
    }
}
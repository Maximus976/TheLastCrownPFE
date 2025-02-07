using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public Light lightningLight; // Assigner la Directional Light dans l’Inspector
    public float minTime = 5f; // Temps minimum entre deux éclairs
    public float maxTime = 15f; // Temps maximum entre deux éclairs
    public float flashIntensity = 2f; // Intensité du flash
    public float flashDuration = 0.1f; // Durée du flash

    private float baseIntensity;

    void Start()
    {
        if (lightningLight == null)
        {
            Debug.LogError("?? Assigne une Directional Light à ThunderFlash !");
            return;
        }
        baseIntensity = 0; // Lumière éteinte en temps normal
        lightningLight.intensity = baseIntensity;
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));

            // Flash rapide (éclair)
            lightningLight.intensity = flashIntensity;
            yield return new WaitForSeconds(flashDuration);

            // Retour à la normale
            lightningLight.intensity = baseIntensity;

            // Petit effet de post-éclair
            if (Random.value > 0.5f) // 50% de chance d'avoir un deuxième flash
            {
                yield return new WaitForSeconds(0.05f);
                lightningLight.intensity = flashIntensity * 0.7f;
                yield return new WaitForSeconds(flashDuration * 0.7f);
                lightningLight.intensity = baseIntensity;
            }
        }
    }
}

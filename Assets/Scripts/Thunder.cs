using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public Light lightningLight; // Assigner la Directional Light dans l�Inspector
    public float minTime = 5f; // Temps minimum entre deux �clairs
    public float maxTime = 15f; // Temps maximum entre deux �clairs
    public float flashIntensity = 2f; // Intensit� du flash
    public float flashDuration = 0.1f; // Dur�e du flash

    private float baseIntensity;

    void Start()
    {
        if (lightningLight == null)
        {
            Debug.LogError("?? Assigne une Directional Light � ThunderFlash !");
            return;
        }
        baseIntensity = 0; // Lumi�re �teinte en temps normal
        lightningLight.intensity = baseIntensity;
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));

            // Flash rapide (�clair)
            lightningLight.intensity = flashIntensity;
            yield return new WaitForSeconds(flashDuration);

            // Retour � la normale
            lightningLight.intensity = baseIntensity;

            // Petit effet de post-�clair
            if (Random.value > 0.5f) // 50% de chance d'avoir un deuxi�me flash
            {
                yield return new WaitForSeconds(0.05f);
                lightningLight.intensity = flashIntensity * 0.7f;
                yield return new WaitForSeconds(flashDuration * 0.7f);
                lightningLight.intensity = baseIntensity;
            }
        }
    }
}

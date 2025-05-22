using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class DamageVignette : MonoBehaviour
{
    public static DamageVignette instance;

    private Volume volume;
    private Vignette vignette;

    private void Awake()
    {
        instance = this;
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
    }

    public void UpdateVignette(float healthPercent)
    {
        if (vignette == null) return;

        // Plus la vie est basse, plus l’intensité augmente
        vignette.intensity.value = Mathf.Lerp(0.5f, 0f, healthPercent);
        vignette.smoothness.value = 1f;
        vignette.color.value = Color.red;
    }
}
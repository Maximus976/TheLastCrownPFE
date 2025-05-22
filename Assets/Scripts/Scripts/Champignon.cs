using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champignon : MonoBehaviour
{
    [Header("Respiration")]
    public float breathSpeed = 1f;           // Vitesse de respiration (oscillation)
    public float scaleAmount = 0.05f;        // Intensité de l'oscillation

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * breathSpeed) * scaleAmount;
        transform.localScale = originalScale * scale;
    }
}
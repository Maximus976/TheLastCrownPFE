using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillation : MonoBehaviour
{
    [Header("Oscillation Settings")]
    public float amplitude = 10f;      // Amplitude de l'oscillation (distance maximale du déplacement)
    public float frequency = 2f;       // Fréquence de l'oscillation (nombre d'oscillations par seconde)

    private Vector3 initialPosition;   // Position initiale du bouton

    void Start()
    {
        // Stocke la position initiale du bouton
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // Calcul de la nouvelle position en Y en fonction du temps
        float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;

        // Mise à jour de la position du bouton
        transform.localPosition = initialPosition + new Vector3(0, offsetY, 0);
    }
}

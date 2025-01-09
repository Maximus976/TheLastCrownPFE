using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalesMoulin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Vitesse de rotation ajustable dans l'inspecteur

    void Update()
    {
        // Fait tourner les pales autour de l'axe Y
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    // Permet de régler la vitesse via un autre script ou une interface
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}


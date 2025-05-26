using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalesMoulin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; 

    void Update()
    {
        // Rotation autour de l’axe X global (de la scène)
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.World);
    }

    // Permet de régler la vitesse via un autre script ou une interface
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}

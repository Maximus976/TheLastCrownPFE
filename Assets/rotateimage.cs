using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateimage : MonoBehaviour
{
    public float rotationSpeed = 50f; // degrés par seconde

    void Update()
    {
        // On tourne autour de l'axe Z en fonction du temps
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
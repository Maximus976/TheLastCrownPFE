using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHealth : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        // Trouver la caméra principale dynamiquement
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Faire en sorte que la barre de vie regarde toujours la caméra
            transform.LookAt(transform.position + mainCamera.forward);
        }
    }
}
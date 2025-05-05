using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDeviceManager : MonoBehaviour
{
    private Vector3 lastMousePosition;
    private bool usingController = false;

    void Update()
    {
        DetectInputDevice();
    }

    private void DetectInputDevice()
    {
        // ➔ Détecte mouvement souris
        if (Input.mousePosition != lastMousePosition)
        {
            SwitchToKeyboardAndMouse();
        }
        lastMousePosition = Input.mousePosition;

        // ➔ Détecte touche clavier
        if (Input.anyKeyDown)
        {
            SwitchToKeyboardAndMouse();
        }

        // ➔ Détecte manette (axes mouvement ou boutons)
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 ||
            Input.GetButtonDown("Sprint") || Input.GetButtonDown("Fire1"))
        {
            SwitchToController();
        }
    }

    private void SwitchToKeyboardAndMouse()
    {
        if (usingController)
        {
            usingController = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            // Debug.Log("Switched to Keyboard & Mouse");
        }
    }

    private void SwitchToController()
    {
        if (!usingController)
        {
            usingController = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            // Debug.Log("Switched to Controller");
        }
    }
}

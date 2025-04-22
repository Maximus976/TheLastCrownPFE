using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevierPillier : MonoBehaviour
{
    public PillierAnim puzzleManager;  // Référence au gestionnaire de piliers
    public int[] affectedPillars;     // Indices des piliers affectés par ce levier
    public Transform leverHandle;      // Référence au levier (pour la rotation)
    public float rotationAngle = -60f; // Angle de rotation du levier lors de l'activation

    private bool isActivated = false;
    private bool canActivate = false;

    void Update()
    {
        if (canActivate && Input.GetKeyDown(KeyCode.E))
        {
            ActivateLever();
        }
    }

    public void ActivateLever()
    {
        isActivated = !isActivated;
        RotateHandle(isActivated);
        Debug.Log("Levier activé, affecte les piliers : " + string.Join(", ", affectedPillars));  // Log pour vérifier les indices
        puzzleManager.TogglePillars(affectedPillars);
    }

    void RotateHandle(bool activate)
    {
        float angle = activate ? rotationAngle : 0f;
        leverHandle.localRotation = Quaternion.Euler(0, angle, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            canActivate = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canActivate = false;
    }
}
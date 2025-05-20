using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevierPillier : MonoBehaviour
{
    public pilliiermanager puzzleManager;  // Référence au gestionnaire
    public int[] affectedPillarsIndices; // Indices des piliers que cette dalle contrôle

    private bool playerOnDalle = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerOnDalle = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerOnDalle = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerOnDalle && other.CompareTag("Player") && !puzzleManager.IsPuzzleSolved)
        {
            puzzleManager.TogglePillars(affectedPillarsIndices);
        }
    }
}
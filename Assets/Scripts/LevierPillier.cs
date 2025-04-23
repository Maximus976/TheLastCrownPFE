using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevierPillier : MonoBehaviour
{
    public PillierAnim puzzleManager;  // Référence au gestionnaire de piliers
    public int[] affectedPillars;      // Indices des piliers affectés par cette dalle

    private bool canActivate = false;   // Si le joueur peut interagir avec la dalle

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canActivate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canActivate = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (canActivate && other.CompareTag("Player") && !puzzleManager.isPuzzleSolved)  // On ne permet pas d'activer les dalles si le puzzle est résolu
        {
            ActivateTile();  // Activer la dalle lorsque le joueur est dessus
        }
    }

    // Activation de la dalle, qui va affecter les piliers
    private void ActivateTile()
    {
        if (puzzleManager == null || puzzleManager.IsAnyPillarMoving())  // Vérifie si des piliers sont en mouvement
        {
            return;  // Ne pas activer si les piliers sont en mouvement
        }

        // Si tout est bon, on active la dalle
        puzzleManager.TogglePillars(affectedPillars);  // Affecte les piliers spécifiés
    }
}
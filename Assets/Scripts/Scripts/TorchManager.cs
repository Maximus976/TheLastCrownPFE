using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class TorchManager : MonoBehaviour
{
    public Torch[] torches;
    public int[] correctOrder;

    private List<int> currentOrder = new List<int>();
    private bool puzzleCompleted = false;

    public AudioSource failSound;
    public AudioSource successSound;
    public AudioSource doorOpenSound;

    public Grille grilleScript; // ? Référence vers le script Grille (pas GameObject directement)

    public void OnTorchInteraction(Torch torch)
    {
        if (puzzleCompleted || torch.IsLit()) return;

        int index = torch.torchIndex;
        int nextExpected = currentOrder.Count < correctOrder.Length ? correctOrder[currentOrder.Count] : -1;

        if (index == nextExpected)
        {
            torch.Light();
            currentOrder.Add(index);

            if (currentOrder.Count == correctOrder.Length)
                CompletePuzzle();
        }
        else
        {
            ResetPuzzle();
        }
    }

    private void CompletePuzzle()
    {
        puzzleCompleted = true;

        if (successSound) successSound.Play();

        foreach (Torch torch in torches)
            torch.Lock();

        if (grilleScript != null)
        {
            grilleScript.OuvrirGrille(); // ? Appelle l’ouverture animée de la grille

            if (doorOpenSound)
                doorOpenSound.Play();
        }
    }

    private void ResetPuzzle()
    {
        if (failSound) failSound.Play();
        currentOrder.Clear();

        foreach (Torch torch in torches)
            torch.Extinguish();
    }
}
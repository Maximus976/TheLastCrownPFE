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
    public AudioSource doorOpenSound; // <-- Nouveau son ajouté ici

    public Animator doorAnimator;
    public string openTriggerName = "Open";

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

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openTriggerName);

            if (doorOpenSound) // <-- On joue le son de la porte ici
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
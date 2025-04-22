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

    public Animator doorAnimator; // Utilise directement la variable ici

    public string openTriggerName = "Open"; // Toujours bon d'avoir cette option

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

        // Utilisation correcte de doorAnimator
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openTriggerName);
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
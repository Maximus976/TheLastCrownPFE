using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldPressButton : MonoBehaviour
{
    [SerializeField] private Image progressImage;  // Image qui représente le cercle de progression

    public void UpdateProgress(float progress)
    {
        progressImage.fillAmount = progress;  // Met à jour le cercle en fonction de la progression
    }

    public void ResetProgress()
    {
        progressImage.fillAmount = 0f;  // Réinitialise le cercle de progression
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldPressButton : MonoBehaviour
{
    [SerializeField] private Image progressImage;  // Image qui repr�sente le cercle de progression

    public void UpdateProgress(float progress)
    {
        progressImage.fillAmount = progress;  // Met � jour le cercle en fonction de la progression
    }

    public void ResetProgress()
    {
        progressImage.fillAmount = 0f;  // R�initialise le cercle de progression
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class PauseMenuItem : MonoBehaviour
{
    [Header("Références")]
    public TMP_Text text;
    public GameObject leafLeft;
    public GameObject leafRight;

    [Header("Couleurs")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    public void SetSelected(bool isSelected)
    {
        if (text != null)
            text.color = isSelected ? selectedColor : normalColor;

        if (leafLeft != null)
            leafLeft.SetActive(isSelected);

        if (leafRight != null)
            leafRight.SetActive(isSelected);
    }
}   
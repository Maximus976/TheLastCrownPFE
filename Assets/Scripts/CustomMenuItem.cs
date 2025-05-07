using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CustomMenuItem
{
    public TMP_Text text;           // Texte TMP de l'�l�ment du menu
    public Button button;           // Le bouton associ� � l'action
    public GameObject leafLeft;     // Feuille gauche
    public GameObject leafRight;    // Feuille droite

    // Met � jour l'�tat de s�lection de l'�l�ment du menu
    public void SetSelected(bool isSelected)
    {
        text.color = isSelected ? Color.yellow : Color.white;  // Change la couleur du texte

        if (leafLeft != null)
            leafLeft.SetActive(isSelected);  // Active ou d�sactive la feuille gauche

        if (leafRight != null)
            leafRight.SetActive(isSelected);  // Active ou d�sactive la feuille droite
    }

    // Fonction appel�e lorsqu'un �l�ment est s�lectionn�
    public void Select()
    {
        if (button != null)
        {
            button.onClick.Invoke();  // Appelle l'action associ�e au bouton
        }
    }
}
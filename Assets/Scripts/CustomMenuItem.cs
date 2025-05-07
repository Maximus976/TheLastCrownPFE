using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CustomMenuItem
{
    public TMP_Text text;           // Texte TMP de l'élément du menu
    public Button button;           // Le bouton associé à l'action
    public GameObject leafLeft;     // Feuille gauche
    public GameObject leafRight;    // Feuille droite

    // Met à jour l'état de sélection de l'élément du menu
    public void SetSelected(bool isSelected)
    {
        text.color = isSelected ? Color.yellow : Color.white;  // Change la couleur du texte

        if (leafLeft != null)
            leafLeft.SetActive(isSelected);  // Active ou désactive la feuille gauche

        if (leafRight != null)
            leafRight.SetActive(isSelected);  // Active ou désactive la feuille droite
    }

    // Fonction appelée lorsqu'un élément est sélectionné
    public void Select()
    {
        if (button != null)
        {
            button.onClick.Invoke();  // Appelle l'action associée au bouton
        }
    }
}
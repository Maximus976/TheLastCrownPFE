using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CustomMenuItem
{
    [Header("Références UI")]
    public TMP_Text text;           // Texte de l'élément du menu
    public Button button;           // Bouton associé
    public GameObject leafLeft;     // Élément visuel à gauche
    public GameObject leafRight;    // Élément visuel à droite

    [Header("Couleurs")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    // Met à jour l'état visuel selon la sélection
    public void SetSelected(bool isSelected)
    {
        if (text != null)
            text.color = isSelected ? selectedColor : normalColor;

        if (leafLeft != null)
            leafLeft.SetActive(isSelected);

        if (leafRight != null)
            leafRight.SetActive(isSelected);
    }

    // Appelle l'action liée au bouton
    public void Select()
    {
        if (button != null)
            button.onClick.Invoke();
    }
}
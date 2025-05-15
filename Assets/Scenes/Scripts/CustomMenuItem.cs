using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CustomMenuItem
{
    [Header("R�f�rences UI")]
    public TMP_Text text;           // Texte de l'�l�ment du menu
    public Button button;           // Bouton associ�
    public GameObject leafLeft;     // �l�ment visuel � gauche
    public GameObject leafRight;    // �l�ment visuel � droite

    [Header("Couleurs")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    // Met � jour l'�tat visuel selon la s�lection
    public void SetSelected(bool isSelected)
    {
        if (text != null)
            text.color = isSelected ? selectedColor : normalColor;

        if (leafLeft != null)
            leafLeft.SetActive(isSelected);

        if (leafRight != null)
            leafRight.SetActive(isSelected);
    }

    // Appelle l'action li�e au bouton
    public void Select()
    {
        if (button != null)
            button.onClick.Invoke();
    }
}
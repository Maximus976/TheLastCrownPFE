using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class PauseMenuItem : MonoBehaviour
{
    public TMP_Text text;          // Référence au texte du bouton
    public Button button;          // Référence au bouton
    public GameObject leafLeft;    // "Leaf" gauche
    public GameObject leafRight;   // "Leaf" droit

    public void SetSelected(bool isSelected)
    {
        text.color = isSelected ? Color.yellow : Color.white;  // Change la couleur du texte

        if (leafLeft != null) leafLeft.SetActive(isSelected);  // Affiche/cacher le "leaf" gauche
        if (leafRight != null) leafRight.SetActive(isSelected);  // Affiche/cacher le "leaf" droit
    }

    public void Select()
    {
        if (button != null)
        {
            button.onClick.Invoke();  // Simule un clic sur le bouton sélectionné
        }
    }
}
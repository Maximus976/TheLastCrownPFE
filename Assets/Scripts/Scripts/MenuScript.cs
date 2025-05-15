using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class MenuScript : MonoBehaviour
{
    public List<CustomMenuItem> menuItems;  // Liste des éléments du menu
    private int currentIndex = 0;           // Index de l'élément sélectionné
    private bool inputLocked = false;       // Empêche les entrées rapides pour éviter des changements trop rapides

    void Start()
    {
        if (menuItems.Count > 0)
        {
            UpdateSelection();  // Met à jour la sélection initiale si la liste n'est pas vide
        }
    }

    void Update()
    {
        // Vérifie que la liste n'est pas vide et que l'index est valide
        if (menuItems.Count == 0)
            return;

        // Récupère l'entrée verticale (joystick ou flèches)
        float vertical = Input.GetAxisRaw("Vertical");

        // Si l'entrée est valide et non verrouillée
        if (!inputLocked && Mathf.Abs(vertical) > 0.5f)
        {
            currentIndex = (vertical < 0)
                ? (currentIndex + 1) % menuItems.Count  // Descendre dans la liste
                : (currentIndex - 1 + menuItems.Count) % menuItems.Count;  // Monter dans la liste

            UpdateSelection();  // Met à jour la sélection visuelle
            StartCoroutine(UnlockInputAfterDelay(0.2f));  // Verrouille les entrées pendant un court délai
        }

        // Utilise GetKeyDown pour détecter la pression d'une touche spécifique (par exemple, le bouton "X" de la manette)
        if (Input.GetKeyDown(KeyCode.JoystickButton1))  // "JoystickButton1" est le bouton pour interagir
        {
            menuItems[currentIndex].Select();  // Sélectionne l'élément actuel
        }
    }

    // Met à jour l'état de sélection de chaque élément
    public void UpdateSelection()
    {
        if (menuItems.Count == 0)
            return;

        // S'assure que l'index actuel est dans la plage de la liste
        currentIndex = Mathf.Clamp(currentIndex, 0, menuItems.Count - 1);

        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].SetSelected(i == currentIndex);  // Change l'apparence de l'élément sélectionné
        }
    }

    // Permet de débloquer l'entrée après un délai
    private IEnumerator UnlockInputAfterDelay(float delay)
    {
        inputLocked = true;
        yield return new WaitForSeconds(delay);
        inputLocked = false;
    }
}
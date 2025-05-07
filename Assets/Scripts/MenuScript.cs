using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class MenuScript : MonoBehaviour
{
    public List<CustomMenuItem> menuItems;  // Liste des �l�ments du menu
    private int currentIndex = 0;           // Index de l'�l�ment s�lectionn�
    private bool inputLocked = false;       // Emp�che les entr�es rapides pour �viter des changements trop rapides

    void Start()
    {
        if (menuItems.Count > 0)
        {
            UpdateSelection();  // Met � jour la s�lection initiale si la liste n'est pas vide
        }
    }

    void Update()
    {
        // V�rifie que la liste n'est pas vide et que l'index est valide
        if (menuItems.Count == 0)
            return;

        // R�cup�re l'entr�e verticale (joystick ou fl�ches)
        float vertical = Input.GetAxisRaw("Vertical");

        // Si l'entr�e est valide et non verrouill�e
        if (!inputLocked && Mathf.Abs(vertical) > 0.5f)
        {
            currentIndex = (vertical < 0)
                ? (currentIndex + 1) % menuItems.Count  // Descendre dans la liste
                : (currentIndex - 1 + menuItems.Count) % menuItems.Count;  // Monter dans la liste

            UpdateSelection();  // Met � jour la s�lection visuelle
            StartCoroutine(UnlockInputAfterDelay(0.2f));  // Verrouille les entr�es pendant un court d�lai
        }

        // Utilise GetKeyDown pour d�tecter la pression d'une touche sp�cifique (par exemple, le bouton "X" de la manette)
        if (Input.GetKeyDown(KeyCode.JoystickButton1))  // "JoystickButton1" est le bouton pour interagir
        {
            menuItems[currentIndex].Select();  // S�lectionne l'�l�ment actuel
        }
    }

    // Met � jour l'�tat de s�lection de chaque �l�ment
    public void UpdateSelection()
    {
        if (menuItems.Count == 0)
            return;

        // S'assure que l'index actuel est dans la plage de la liste
        currentIndex = Mathf.Clamp(currentIndex, 0, menuItems.Count - 1);

        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].SetSelected(i == currentIndex);  // Change l'apparence de l'�l�ment s�lectionn�
        }
    }

    // Permet de d�bloquer l'entr�e apr�s un d�lai
    private IEnumerator UnlockInputAfterDelay(float delay)
    {
        inputLocked = true;
        yield return new WaitForSeconds(delay);
        inputLocked = false;
    }
}
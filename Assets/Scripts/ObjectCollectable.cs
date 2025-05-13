using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectCollectable : MonoBehaviour
{
    public string itemDescription;  // Description de l'objet
    public static int itemsCollected = 0;  // Nombre d'objets collect�s
    public static int totalItems = 4;  // Nombre total d'objets collectables
    public GameObject popUpPanel;  // Panel du pop-up (toujours actif)
    public TMP_Text itemDescriptionText;  // TMP_Text pour afficher la description de l'objet
    public TMP_Text itemsCollectedText;  // TMP_Text pour afficher le nombre d'objets collect�s
    private bool playerInRange = false;  // Flag pour savoir si le joueur est dans la zone

    private void OnTriggerEnter(Collider other)
    {
        // Quand le joueur entre dans la zone de d�tection
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Quand le joueur quitte la zone de d�tection
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        // Si le joueur est dans la zone de d�tection et appuie sur JoystickButton2
        if (playerInRange && Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        // Afficher la description de l'objet via TMP_Text
        itemDescriptionText.text = itemDescription;

        // Afficher le nombre d'objets collect�s
        itemsCollected++;
        itemsCollectedText.text = $"{itemsCollected}/{totalItems} objets r�cup�r�s";

        // Afficher le pop-up et mettre le jeu en pause
        popUpPanel.SetActive(true);
        Time.timeScale = 0;

        // D�sactiver l'objet collectable
        gameObject.SetActive(false);
    }

    public void ClosePopUp()
    {
        // Fermer le pop-up et relancer le jeu
        popUpPanel.SetActive(false);
        Time.timeScale = 1;  // Relancer le jeu � l'endroit o� il �tait
    }
}
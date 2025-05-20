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

    public static List<GameObject> collectedObjects = new List<GameObject>(); // Liste des objets collect�s

    private bool playerInRange = false;  // Flag pour savoir si le joueur est dans la zone

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        // Affiche la description de l'objet
        itemDescriptionText.text = itemDescription;

        // Incr�mente et affiche le nombre d'objets collect�s
        itemsCollected++;
        itemsCollectedText.text = $"{itemsCollected}/{totalItems} objets";

        // Ajoute l'objet � la liste des objets collect�s
        collectedObjects.Add(gameObject);

        // Affiche le pop-up et met le jeu en pause
        popUpPanel.SetActive(true);
        Time.timeScale = 0f;

        // D�sactive l'objet
        gameObject.SetActive(false);
    }

    public void ClosePopUp()
    {
        // Ferme le pop-up et relance le jeu
        popUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
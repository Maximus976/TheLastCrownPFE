using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectCollectable : MonoBehaviour
{
    public string itemDescription;  // Description de l'objet
    public static int itemsCollected = 0;
    public static int totalItems = 4;
    public GameObject popUpPanel;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemsCollectedText;

    public GameObject detectionZone;  // ? Zone de détection à détruire

    public static List<GameObject> collectedObjects = new List<GameObject>();

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
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
        itemDescriptionText.text = itemDescription;
        itemsCollected++;
        itemsCollectedText.text = $"{itemsCollected}/{totalItems} objets";
        collectedObjects.Add(gameObject);

        popUpPanel.SetActive(true);
        Time.timeScale = 0f;

        if (detectionZone != null) Destroy(detectionZone); // ? Supprime la zone de détection

        gameObject.SetActive(false);
    }

    public void ClosePopUp()
    {
        popUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // --- Méthode pour réinitialiser le comptage et la liste ---
    public static void ResetCollectables()
    {
        itemsCollected = 0;
        collectedObjects.Clear();
    }
}
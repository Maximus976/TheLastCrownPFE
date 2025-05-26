using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pancarte : MonoBehaviour
{
    [TextArea]
    public string message; // Le message à afficher
    public GameObject popUpPanel;
    public TMP_Text messageText;

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
            ShowMessage();
        }
    }

    void ShowMessage()
    {
        if (popUpPanel != null && messageText != null)
        {
            messageText.text = message;
            popUpPanel.SetActive(true);
            Time.timeScale = 0f; // Pause le jeu pendant la lecture du message
        }
    }

    public void CloseMessage()
    {
        if (popUpPanel != null)
        {
            popUpPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
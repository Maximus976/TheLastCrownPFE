using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Statue : MonoBehaviour
{
    public GameObject narrationPanel;
    public TMP_Text narrationText;
    public Image holdCircle;
    public float holdDuration = 2f;
    public GameObject grille;

    private bool playerInZone = false;
    private bool eventTriggered = false;
    private float holdTimer = 0f;
    private bool panelActive = false;
    private bool isDepositing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            CancelDeposit();
        }
    }

    void Update()
    {
        // Si le joueur est dans la zone, n’a pas déjà déposé, et tous les objets sont collectés
        if (playerInZone && !eventTriggered && ObjectCollectable.itemsCollected >= ObjectCollectable.totalItems)
        {
            // Si le joueur appuie sur le bouton, lancer le dépôt
            if (Input.GetKeyDown(KeyCode.JoystickButton2) && !panelActive)
            {
                ShowNarration();
                isDepositing = true;
            }

            // Si on est en train de déposer (bouton appuyé en continu)
            if (isDepositing && panelActive)
            {
                if (Input.GetKey(KeyCode.JoystickButton2))
                {
                    holdTimer += Time.unscaledDeltaTime;
                    holdCircle.fillAmount = holdTimer / holdDuration;

                    if (holdTimer >= holdDuration)
                    {
                        CloseNarrationAndOpenGrille();
                    }
                }
                else
                {
                    // Si on relâche trop tôt
                    holdTimer = 0f;
                    holdCircle.fillAmount = 0f;
                }
            }
        }
    }

    void ShowNarration()
    {
        panelActive = true;
        narrationPanel.SetActive(true);
        narrationText.text = "Vous avez restauré les fragments perdus...\nLe chemin s’ouvre à nouveau.";
        Time.timeScale = 0f;
        holdTimer = 0f;
        holdCircle.fillAmount = 0f;
    }

    void CloseNarrationAndOpenGrille()
    {
        Time.timeScale = 1f;
        narrationPanel.SetActive(false);

        if (grille != null)
        {
            Grille grilleScript = grille.GetComponent<Grille>();
            if (grilleScript != null)
            {
                grilleScript.OuvrirGrille();
            }
        }

        eventTriggered = true;
        panelActive = false;
        isDepositing = false;
    }

    void CancelDeposit()
    {
        if (panelActive)
        {
            narrationPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        holdTimer = 0f;
        holdCircle.fillAmount = 0f;
        panelActive = false;
        isDepositing = false;
    }
}
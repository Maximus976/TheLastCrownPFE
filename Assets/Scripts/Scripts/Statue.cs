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

    public List<Transform> dropPoints; // Liste de positions fixes

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
        if (playerInZone && !eventTriggered && ObjectCollectable.itemsCollected >= ObjectCollectable.totalItems)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton2) && !panelActive)
            {
                ShowNarration();
                isDepositing = true;
            }

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

        // Redéposer les objets à leurs points définis
        for (int i = 0; i < ObjectCollectable.collectedObjects.Count; i++)
        {
            GameObject obj = ObjectCollectable.collectedObjects[i];
            if (obj != null && i < dropPoints.Count)
            {
                obj.transform.position = dropPoints[i].position;
                obj.transform.rotation = dropPoints[i].rotation;
                obj.SetActive(true);

                // Désactive le collider
                Collider col = obj.GetComponent<Collider>();
                if (col) col.enabled = false;

                // Désactive le script de collecte
                ObjectCollectable script = obj.GetComponent<ObjectCollectable>();
                if (script) Destroy(script);

                // Désactive l'oscillation
                OcciliationObjet floatScript = obj.GetComponent<OcciliationObjet>();
                if (floatScript) floatScript.DisableFloating();
            }
        }
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
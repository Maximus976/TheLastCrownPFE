using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Statue : MonoBehaviour
{
    [Header("Interaction et UI")]
    public GameObject missingPanel;      // Panel affiché si objets manquants
    public TMP_Text missingText;

    [Header("Références")]
    public GameObject grille;            // La grille à ouvrir
    public Transform statueModel;        // Le modèle 3D de la statue à secouer/descendre
    public List<Transform> dropPoints;   // Positions pour déposer les objets
    public GameObject detectionZone;     // ? Zone de détection à supprimer après interaction

    [Header("Audio")]
    public AudioSource shakeSound;       // Son de secousse
    public AudioSource descendSound;     // Son pendant la descente

    [Header("Effets")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
    public float descendTargetY = -8.07f;
    public float descendDuration = 2f;

    private bool playerInZone = false;
    private bool eventTriggered = false;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            if (eventTriggered) return;

            if (ObjectCollectable.itemsCollected >= ObjectCollectable.totalItems)
            {
                StartCoroutine(HandleStatueEvent());
            }
            else
            {
                ShowMissingPanel("La statue reste silencieuse. Il vous manque des objets.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = false;
    }

    IEnumerator HandleStatueEvent()
    {
        eventTriggered = true;

        // ? Supprimer la zone de détection si définie
        if (detectionZone != null)
            Destroy(detectionZone);

        // ? Déposer les objets collectés aux bons emplacements
        for (int i = 0; i < ObjectCollectable.collectedObjects.Count; i++)
        {
            GameObject obj = ObjectCollectable.collectedObjects[i];
            if (obj != null && i < dropPoints.Count)
            {
                obj.transform.position = dropPoints[i].position;
                obj.transform.rotation = dropPoints[i].rotation;
                obj.SetActive(true);

                var col = obj.GetComponent<Collider>();
                if (col) col.enabled = false;

                var script = obj.GetComponent<ObjectCollectable>();
                if (script) Destroy(script);

                var floatScript = obj.GetComponent<OcciliationObjet>();
                if (floatScript) floatScript.DisableFloating();
            }
        }

        // ? Jouer le son de secousse
        if (shakeSound) shakeSound.Play();

        // ? Animation de secousse
        Vector3 originalPos = statueModel.position;
        float timer = 0f;
        while (timer < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetZ = Random.Range(-1f, 1f) * shakeMagnitude;
            statueModel.position = originalPos + new Vector3(offsetX, 0f, offsetZ);
            timer += Time.deltaTime;
            yield return null;
        }
        statueModel.position = originalPos;

        // ? Jouer le son de descente
        if (descendSound) descendSound.Play();

        // ? Descente fluide
        Vector3 start = statueModel.position;
        Vector3 target = new Vector3(start.x, descendTargetY, start.z);
        float t = 0f;
        while (t < descendDuration)
        {
            statueModel.position = Vector3.Lerp(start, target, t / descendDuration);
            t += Time.deltaTime;
            yield return null;
        }
        statueModel.position = target;

        // ? Ouverture de la grille
        if (grille != null)
        {
            Grille grilleScript = grille.GetComponent<Grille>();
            if (grilleScript != null)
            {
                grilleScript.OuvrirGrille();
            }
        }
    }

    void ShowMissingPanel(string message)
    {
        if (missingPanel && missingText)
        {
            missingPanel.SetActive(true);
            missingText.text = message;
            Time.timeScale = 0f; // ? Pause du jeu
        }
    }

    public void CloseMissingPanel()
    {
        if (missingPanel)
        {
            missingPanel.SetActive(false);
            Time.timeScale = 1f; // ? Reprise du jeu
        }
    }
}
using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject interactionUI; // une UI pour "Appuyez sur E"
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<CameraManager>().ShowDoorSequence(); // Appel de la caméra
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactionUI.SetActive(false);
        }
    }
}

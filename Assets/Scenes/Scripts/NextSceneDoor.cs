using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneDoor : MonoBehaviour
{
    public string teleportPointName = "BouclierSpawn";
    public Canvas loadingCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportAfterDelay(other.gameObject));
        }
    }

    IEnumerator TeleportAfterDelay(GameObject player)
    {
        // Afficher le canvas
        loadingCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        // Trouver le point de t�l�portation
        TeleportPoint[] points = FindObjectsOfType<TeleportPoint>();
        foreach (var point in points)
        {
            if (point.spawnPointName == teleportPointName)
            {
                // D�sactiver temporairement le CharacterController si pr�sent
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc) cc.enabled = false;

                // D�placer le joueur
                player.transform.position = point.transform.position;
                player.transform.rotation = point.transform.rotation;

                // R�initialiser la vitesse si Rigidbody
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // R�activer le CharacterController apr�s d�placement
                if (cc) cc.enabled = true;

                break;
            }
        }

        // Cacher le canvas
        loadingCanvas.gameObject.SetActive(false);
    }
}
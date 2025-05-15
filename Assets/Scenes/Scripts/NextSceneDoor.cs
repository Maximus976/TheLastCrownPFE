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

        // Trouver le point de téléportation
        TeleportPoint[] points = FindObjectsOfType<TeleportPoint>();
        foreach (var point in points)
        {
            if (point.spawnPointName == teleportPointName)
            {
                // Désactiver temporairement le CharacterController si présent
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc) cc.enabled = false;

                // Déplacer le joueur
                player.transform.position = point.transform.position;
                player.transform.rotation = point.transform.rotation;

                // Réinitialiser la vitesse si Rigidbody
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // Réactiver le CharacterController après déplacement
                if (cc) cc.enabled = true;

                break;
            }
        }

        // Cacher le canvas
        loadingCanvas.gameObject.SetActive(false);
    }
}
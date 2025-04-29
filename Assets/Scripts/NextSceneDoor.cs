using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneDoor : MonoBehaviour
{
    public string sceneToLoad = "Tutoriel 4.0";  // Le nom de la scène que tu veux charger après le chargement

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Charge la scène de chargement
            Loading.LoadSceneWithLoading(sceneToLoad);
        }
    }
}
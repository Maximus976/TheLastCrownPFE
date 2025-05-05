using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneDoor : MonoBehaviour
{
    public string sceneToLoad = "Tutoriel 4.0";  // Le nom de la sc�ne que tu veux charger apr�s le chargement

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Charge la sc�ne de chargement
            Loading.LoadSceneWithLoading(sceneToLoad);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static string targetSpawnPoint = null;

    // Méthode statique pour charger la scène via l'écran de chargement
    public static void LoadSceneWithLoading(string sceneName, string spawnPointName)
    {
        targetSpawnPoint = spawnPointName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
        // La scène "Loading" chargera ensuite sceneName
        PlayerPrefs.SetString("SceneToLoad", sceneName); // stock temporaire
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static string targetSpawnPoint = null;

    // M�thode statique pour charger la sc�ne via l'�cran de chargement
    public static void LoadSceneWithLoading(string sceneName, string spawnPointName)
    {
        targetSpawnPoint = spawnPointName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
        // La sc�ne "Loading" chargera ensuite sceneName
        PlayerPrefs.SetString("SceneToLoad", sceneName); // stock temporaire
    }
}
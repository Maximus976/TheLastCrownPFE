using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static string targetScene;  // Scène à charger après l'écran de chargement

    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        yield return new WaitForSeconds(2f);  // Temps de l'écran de chargement (ajuste à ton goût)

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);  // Charge la scène suivante
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Cette méthode est appelée pour démarrer le processus de chargement
    public static void LoadSceneWithLoading(string sceneName)
    {
        targetScene = sceneName;  // Définit la scène à charger après l'écran de chargement
        SceneManager.LoadScene("Loading");  // Charge la scène de chargement
    }
}
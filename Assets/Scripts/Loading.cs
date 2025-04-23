using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static string targetScene;  // Sc�ne � charger apr�s l'�cran de chargement

    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        yield return new WaitForSeconds(2f);  // Temps de l'�cran de chargement (ajuste � ton go�t)

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);  // Charge la sc�ne suivante
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Cette m�thode est appel�e pour d�marrer le processus de chargement
    public static void LoadSceneWithLoading(string sceneName)
    {
        targetScene = sceneName;  // D�finit la sc�ne � charger apr�s l'�cran de chargement
        SceneManager.LoadScene("Loading");  // Charge la sc�ne de chargement
    }
}
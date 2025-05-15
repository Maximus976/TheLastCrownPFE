using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FakeLoadingTeleport : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(FakeLoadRoutine());
    }

    IEnumerator FakeLoadRoutine()
    {
        yield return new WaitForSeconds(2f); // Simulation du chargement

        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "Tutoriel_Stable");
        SceneManager.LoadScene(sceneToLoad);
    }
}
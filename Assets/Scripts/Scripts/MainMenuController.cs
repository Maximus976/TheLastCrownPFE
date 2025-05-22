using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject optionsMenu;  // Menu des options
    public GameObject mainMenu;     // Menu principal

    // Démarre le jeu
    public void StartGame()
    {
        Debug.Log("Démarrer le jeu !");

        // RESET des objets collectés avant de commencer une nouvelle partie
        ObjectCollectable.ResetCollectables();

        SceneManager.LoadScene("Intro1");  // Charge ta scène de jeu
    }

    // Ouvre le menu des options
    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // Quitte le jeu
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

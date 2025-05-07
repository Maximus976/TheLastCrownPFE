using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject optionsMenu;  // Menu des options
    public GameObject mainMenu;     // Menu principal

    // D�marre le jeu
    public void StartGame()
    {
        Debug.Log("D�marrer le jeu !");
        // Code pour d�marrer le jeu ici...
        SceneManager.LoadScene("Intro1");  // Charge ta sc�ne de jeu
    }

    // Ouvre le menu des options
    public void OpenOptions()
    {
        mainMenu.SetActive(false);  // D�sactive le menu principal
        optionsMenu.SetActive(true);  // Active le menu des options
    }

    // Quitte le jeu
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu.");
        Application.Quit();  // Quitte l'application
    }
}
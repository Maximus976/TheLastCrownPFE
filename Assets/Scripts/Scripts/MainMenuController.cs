using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Menus")]
    public GameObject optionsMenu;  // Menu des options
    public GameObject mainMenu;     // Menu principal

    [Header("Audio")]
    public AudioSource navigateAudioSource;  // Son pour naviguer
    public AudioSource selectAudioSource;    // Son pour valider / lancer

    // Démarre le jeu
    public void StartGame()
    {
        Debug.Log("Démarrer le jeu !");
        PlaySelectSound();

        // RESET des objets collectés avant de commencer une nouvelle partie
        ObjectCollectable.ResetCollectables();

        SceneManager.LoadScene("Intro1");  // Charge ta scène de jeu
    }

    // Ouvre le menu des options
    public void OpenOptions()
    {
        Debug.Log("Ouvrir le menu des options");
        PlayNavigateSound();

        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // Quitte le jeu
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu.");
        PlaySelectSound();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void PlayNavigateSound()
    {
        if (navigateAudioSource != null)
            navigateAudioSource.Play();
    }

    private void PlaySelectSound()
    {
        if (selectAudioSource != null)
            selectAudioSource.Play();
    }
}
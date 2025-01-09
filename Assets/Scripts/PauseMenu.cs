using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;
    public GameObject optionsPanel;
    public Slider volumeSlider;
    public Text volumeText;
    public Button backButton;

    [Header("Audio Settings")]
    public AudioSource audioSource;

    private bool isPaused = false;

    void Start()
    {
        // Assurez-vous que le menu de pause est désactivé au départ
        pauseMenuUI.SetActive(false);

        // Ajouter des écouteurs d'événements aux boutons
        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OnOptionsButtonClicked);  // Ajouter le bouton des options
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        quitButton.onClick.AddListener(QuitGame);
        backButton.onClick.AddListener(OnBackButtonClicked);  // Ajouter le bouton de retour

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Initialiser le panneau des options et le slider
        optionsPanel.SetActive(false);
        volumeSlider.value = audioSource.volume;
        volumeText.text = "Volume: " + Mathf.RoundToInt(audioSource.volume * 100).ToString() + "%";
    }

    private void OnOptionsButtonClicked()
    {
        Debug.Log("Options Button Clicked!");
        // Passer du menu principal aux options
        pauseMenuUI.SetActive(false);
        optionsPanel.SetActive(true);
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("Quit Button Clicked!");
        Application.Quit();
    }

    private void OnBackButtonClicked()
    {
        // Revenir au menu principal depuis le panel des options
        optionsPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    private void OnVolumeChanged(float value)
    {
        // Met à jour le texte du volume en fonction du slider
        volumeText.text = "Volume: " + Mathf.RoundToInt(value * 100).ToString() + "%";

        // Met à jour le volume de l'AudioSource
        audioSource.volume = value;
    }

    void Update()
    {
        // Vérifie si la touche 'Échap' est pressée pour ouvrir/fermer le menu de pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // Fonction pour mettre en pause le jeu
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;  // Arrête le temps (le jeu est mis en pause)
        isPaused = true;
    }

    // Fonction pour reprendre le jeu
    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;  // Reprend le temps (le jeu reprend)
        isPaused = false;
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;  // Reprend le temps avant de revenir au menu principal
        SceneManager.LoadScene("Menu");  // Charger la scène du menu principal
    }

    // Fonction pour quitter le jeu
    void QuitGame()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();  // Ferme l'application
    }
}
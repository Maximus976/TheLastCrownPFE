using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainMenuPanel;  // Le panel principal du menu
    public GameObject optionsPanel;   // Le panel des options

    public Slider volumeSlider;       // Le slider pour régler le volume
    public Text volumeText;           // Le texte affichant le volume actuel
    public Button playButton;         // Le bouton pour démarrer le jeu
    public Button optionsButton;      // Le bouton pour ouvrir les options
    public Button quitButton;         // Le bouton pour quitter le jeu
    public Button backButton;         // Le bouton pour revenir au menu principal
  


    [Header("Audio Settings")]
    public AudioSource audioSource;   // L'AudioSource à contrôler (assurez-vous qu'il est assigné dans l'inspecteur)

 

    private void Start()
    {
        // Initialisation des événements
        playButton.onClick.AddListener(OnPlayButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Initialisation des panels
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        volumeSlider.value = audioSource.volume;
        volumeText.text = "Volume: " + Mathf.RoundToInt(audioSource.volume * 100).ToString() + "%";
    }

    public void Update()
    {

    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("Play Button Clicked!");
        // Charger la scène suivante
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnOptionsButtonClicked()
    {
        Debug.Log("Options Button Clicked!");
        // Passer du menu principal aux options
        mainMenuPanel.SetActive(false);
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
        mainMenuPanel.SetActive(true);
    }



    private void OnMainMenuButtonClicked()
    {
        Debug.Log("Main Menu Button Clicked!");
        // Charger la scène du menu principal (vérifiez que le nom de la scène est correct)
        SceneManager.LoadScene("Menu");
    }

    private void OnVolumeChanged(float value)
    {
        // Met à jour le texte du volume en fonction du slider
        volumeText.text = "Volume: " + Mathf.RoundToInt(value * 100).ToString() + "%";

        // Met à jour le volume de l'AudioSource
        audioSource.volume = value;
    }
}

   
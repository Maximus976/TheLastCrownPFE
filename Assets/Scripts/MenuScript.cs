using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuScript : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainMenuPanel;  // Le panel principal du menu
    public GameObject optionsPanel;   // Le panel des options

    public Slider volumeSlider;       // Le slider pour régler le volume
    public Text volumeText;           // Le texte affichant le volume actuel
    public Slider brightnessSlider;   // Le slider pour régler la luminosité
    public Text brightnessText;       // Le texte affichant la luminosité actuelle
    public Button playButton;         // Le bouton pour démarrer le jeu
    public Button optionsButton;      // Le bouton pour ouvrir les options
    public Button quitButton;         // Le bouton pour quitter le jeu
    public Button backButton;         // Le bouton pour revenir au menu principal
    public Image fadeImage;
    public float fadeDuration = 1.0f; // Durée du fondu

    [Header("Audio Settings")]
    public AudioSource audioSource;   // L'AudioSource à contrôler (assurez-vous qu'il est assigné dans l'inspecteur)

    [Header("Lighting Settings")]
    public Light mainLight;           // La lumière principale à contrôler (assurez-vous qu'elle est assignée dans l'inspecteur)

    private void Start()
    {
        // Initialisation des événements
        playButton.onClick.AddListener(OnPlayButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);

        // Initialisation des panels
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // Initialisation des sliders et des textes
        volumeSlider.value = audioSource.volume;
        volumeText.text = "Volume: " + Mathf.RoundToInt(audioSource.volume * 100).ToString() + "%";

        if (mainLight != null)
        {
            brightnessSlider.value = mainLight.intensity;
            brightnessText.text = "Luminosité: " + Mathf.RoundToInt(mainLight.intensity * 100).ToString() + "%";
        }

        if (fadeImage != null)
        {
            // Assurez-vous que l'image est entièrement transparente au démarrage
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("Play Button Clicked!");
        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        // D'abord, fade out progressif de la musique
        if (audioSource != null)
        {
            float startVolume = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float ratio = t / fadeDuration;
                audioSource.volume = Mathf.Lerp(startVolume, 0, ratio);
                yield return null;
            }
            audioSource.volume = 0;
        }

        // Ensuite, fade out de l'écran (augmentation de l'alpha à 1)
        if (fadeImage != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float alpha = t / fadeDuration;
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        // Charger la scène suivante
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        yield return null;
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

    private void OnVolumeChanged(float value)
    {
        // Met à jour le texte du volume en fonction du slider
        volumeText.text = "Volume: " + Mathf.RoundToInt(value * 100).ToString() + "%";

        // Met à jour le volume de l'AudioSource
        audioSource.volume = value;
    }

    private void OnBrightnessChanged(float value)
    {
        if (mainLight != null)
        {
            // Met à jour l'intensité de la lumière principale
            mainLight.intensity = value;

            // Met à jour le texte de la luminosité en fonction du slider
            brightnessText.text = "Luminosité: " + Mathf.RoundToInt(value * 100).ToString() + "%";
        }
    }
}

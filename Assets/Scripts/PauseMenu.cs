using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuUI;
    public GameObject optionsPanel;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;
    public Slider volumeSlider;
    public Text volumeText;
    public Button backButton;
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("Camera Settings")]
    public Camera mainCamera;
    public Vector3 pauseCameraPosition = new Vector3(0f, 10f, -10f);
    public Vector3 pauseCameraRotation = new Vector3(45f, 0f, 0f);
    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;

    private bool isPaused = false;

    [Header("Buttons Settings")]
    public List<Button> buttons; // Liste des boutons
    public Color normalColor = Color.white; // Couleur par défaut
    public Color hoverColor = Color.gray; // Couleur au survol
    public float transitionSpeed = 5f; // Vitesse de transition des couleurs

    private Dictionary<Button, Image> buttonImages = new Dictionary<Button, Image>();
    private Dictionary<Button, bool> isHovering = new Dictionary<Button, bool>();

    void Start()
    {
        // Initialisation des boutons
        foreach (var button in buttons)
        {
            if (button == null) continue;

            var buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImages[button] = buttonImage;
                isHovering[button] = false;
                buttonImage.color = normalColor;

                var eventTrigger = button.gameObject.AddComponent<EventTrigger>();

                // PointerEnter
                var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                pointerEnter.callback.AddListener((eventData) => OnPointerEnter(button));
                eventTrigger.triggers.Add(pointerEnter);

                // PointerExit
                var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                pointerExit.callback.AddListener((eventData) => OnPointerExit(button));
                eventTrigger.triggers.Add(pointerExit);
            }
            else
            {
                Debug.LogWarning($"Aucun composant Image trouvé sur le bouton {button.name} !");
            }
        }

        // Désactivation des panneaux au démarrage
        pauseMenuUI.SetActive(false);
        optionsPanel.SetActive(false);

        // Configuration des boutons
        resumeButton.onClick.AddListener(() => { Resume(); ClearButtonSelection(); });
        settingsButton.onClick.AddListener(() => { OpenOptions(); ClearButtonSelection(); });
        mainMenuButton.onClick.AddListener(() => { GoToMainMenu(); ClearButtonSelection(); });
        quitButton.onClick.AddListener(() => { QuitGame(); ClearButtonSelection(); });
        backButton.onClick.AddListener(() => { CloseOptions(); ClearButtonSelection(); });

        // Initialisation du volume
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        volumeSlider.value = audioSource.volume;
        volumeText.text = "Volume: " + Mathf.RoundToInt(audioSource.volume * 100) + "%";

        // Sauvegarde de la position et de la rotation de la caméra
        if (mainCamera != null)
        {
            initialCameraPosition = mainCamera.transform.position;
            initialCameraRotation = mainCamera.transform.rotation;
        }

        // Fade-in initial
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }

        // Gestion des couleurs des boutons
        foreach (var button in buttons)
        {
            if (buttonImages.ContainsKey(button))
            {
                var targetColor = isHovering[button] ? hoverColor : normalColor;
                buttonImages[button].color = Color.Lerp(buttonImages[button].color, targetColor, Time.unscaledDeltaTime * transitionSpeed);
            }
        }

        // Désélection automatique des boutons si la souris n'est pas dessus
        if (EventSystem.current.currentSelectedGameObject != null && !IsPointerOverUIElement())
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    void Pause()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause le jeu, mais pas l'UI
        UpdateCameraPosition(true);
    }

    void Resume()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;
        UpdateCameraPosition(false);
    }

    void OpenOptions()
    {
        pauseMenuUI.SetActive(false);
        optionsPanel.SetActive(true);
    }

    void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    void GoToMainMenu()
    {
        StartCoroutine(FadeOutAndLoadScene("Menu"));
        Time.timeScale = 1f;
    }

    private void UpdateCameraPosition(bool isPaused)
    {
        if (mainCamera != null)
        {
            if (isPaused)
            {
                mainCamera.transform.position = pauseCameraPosition;
                mainCamera.transform.rotation = Quaternion.Euler(pauseCameraRotation);
            }
            else
            {
                mainCamera.transform.position = initialCameraPosition;
                mainCamera.transform.rotation = initialCameraRotation;
            }
        }
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (fadeImage != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                float alpha = t / fadeDuration;
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            fadeImage.color = new Color(0, 0, 0, 1);
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                float alpha = 1 - (t / fadeDuration);
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    void OnVolumeChanged(float value)
    {
        audioSource.volume = value;
        volumeText.text = "Volume: " + Mathf.RoundToInt(value * 100) + "%";
    }

    void QuitGame()
    {
        Application.Quit();
    }

    private void OnPointerEnter(Button button)
    {
        if (isHovering.ContainsKey(button))
        {
            isHovering[button] = true;
        }
    }

    private void OnPointerExit(Button button)
    {
        if (isHovering.ContainsKey(button))
        {
            isHovering[button] = false;
        }
    }

    private void ClearButtonSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
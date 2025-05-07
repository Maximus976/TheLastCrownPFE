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

    [Header("Buttons Settings")]
    public List<Button> buttons;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.gray;
    public float transitionSpeed = 5f;

    private Dictionary<Button, Image> buttonImages = new Dictionary<Button, Image>();
    private Dictionary<Button, bool> isHovering = new Dictionary<Button, bool>();

    private bool isPaused = false;

    void Start()
    {
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

                var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                pointerEnter.callback.AddListener((eventData) => OnPointerEnter(button));
                eventTrigger.triggers.Add(pointerEnter);

                var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                pointerExit.callback.AddListener((eventData) => OnPointerExit(button));
                eventTrigger.triggers.Add(pointerExit);
            }
        }

        pauseMenuUI.SetActive(false);
        optionsPanel.SetActive(false);

        resumeButton.onClick.AddListener(() => { Resume(); ClearButtonSelection(); });
        settingsButton.onClick.AddListener(() => { OpenOptions(); ClearButtonSelection(); });
        mainMenuButton.onClick.AddListener(() => { GoToMainMenu(); ClearButtonSelection(); });
        quitButton.onClick.AddListener(() => { QuitGame(); ClearButtonSelection(); });
        backButton.onClick.AddListener(() => { CloseOptions(); ClearButtonSelection(); });

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        volumeSlider.value = audioSource.volume;
        volumeText.text = "Volume: " + Mathf.RoundToInt(audioSource.volume * 100) + "%";

        if (mainCamera != null)
        {
            initialCameraPosition = mainCamera.transform.position;
            initialCameraRotation = mainCamera.transform.rotation;
        }

        StartCoroutine(FadeFromBlack());

        var nav = volumeSlider.navigation;
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnDown = backButton;
        volumeSlider.navigation = nav;

        var backNav = backButton.navigation;
        backNav.mode = Navigation.Mode.Explicit;
        backNav.selectOnUp = volumeSlider;
        backButton.navigation = backNav;

        volumeSlider.wholeNumbers = false;
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = audioSource.volume;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            if (isPaused) Resume();
            else Pause();
        }

        if (isPaused && Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(selected, pointer, ExecuteEvents.submitHandler);
            }
        }

        foreach (var button in buttons)
        {
            if (buttonImages.ContainsKey(button))
            {
                var targetColor = isHovering[button] || button.gameObject == EventSystem.current.currentSelectedGameObject ? hoverColor : normalColor;
                buttonImages[button].color = Color.Lerp(buttonImages[button].color, targetColor, Time.unscaledDeltaTime * transitionSpeed);
            }
        }

        if (EventSystem.current.currentSelectedGameObject == null && buttons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }
    }

    void Pause()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        UpdateCameraPosition(true);
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
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
        StartCoroutine(SelectSliderNextFrame());
    }

    private IEnumerator SelectSliderNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
    }

    void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
    }

    void GoToMainMenu()
    {
        StartCoroutine(FadeOutAndLoadScene("Menu"));
        Time.timeScale = 1f;
    }

    void QuitGame()
    {
        Application.Quit();
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
            fadeImage.gameObject.SetActive(true);

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

    private IEnumerator FadeFromBlack()
    {
        if (fadeImage != null)
        {
            // Démarre avec l'image complètement noire
            fadeImage.color = new Color(0, 0, 0, 1);
            fadeImage.gameObject.SetActive(true);

            // Attend 2 secondes avant de commencer le fade
            yield return new WaitForSecondsRealtime(2f);

            // Fade vers transparent
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                float alpha = 1 - (t / fadeDuration);
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            // Assure que l'image est complètement transparente et la désactive
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }
    }

    void OnVolumeChanged(float value)
    {
        audioSource.volume = value;
        volumeText.text = "Volume: " + Mathf.RoundToInt(value * 100) + "%";
    }

    private void OnPointerEnter(Button button)
    {
        if (isHovering.ContainsKey(button))
        {
            isHovering[button] = true;
            EventSystem.current.SetSelectedGameObject(button.gameObject);
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
}

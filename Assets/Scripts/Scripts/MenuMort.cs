using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class MenuMort : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameOverUI gameOverUI;
    public GameObject[] menuButtons;
    public GameObject leafPrefab;
    public UnityEngine.Rendering.Volume globalVolume;
    public string sceneRetryName = "Tutoriel_Stable";

    private int currentIndex = 0;
    private bool isPaused = false;
    private bool inputLocked = false;
    private GameObject currentLeaf;
    private PauseMenuItem[] menuItems;

    [Header("Audio")]
    public AudioSource navigateAudioSource;
    public AudioSource selectAudioSource;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        currentLeaf = Instantiate(leafPrefab);
        currentLeaf.SetActive(false);

        menuItems = new PauseMenuItem[menuButtons.Length];
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuItems[i] = menuButtons[i].GetComponent<PauseMenuItem>();
            menuButtons[i].SetActive(false);
        }

        if (globalVolume != null)
            globalVolume.enabled = false;

        UpdateSelection();
    }

    void Update()
    {
        if (!isPaused) return;

        float vertical = Input.GetAxisRaw("Vertical");
        if (!inputLocked && Mathf.Abs(vertical) > 0.5f)
        {
            int previousIndex = currentIndex;

            if (vertical < 0)
                currentIndex = (currentIndex + 1) % menuButtons.Length;
            else
                currentIndex = (currentIndex - 1 + menuButtons.Length) % menuButtons.Length;

            if (previousIndex != currentIndex)
            {
                PlayNavigateSound();
                UpdateSelection();
            }

            StartCoroutine(UnlockInputAfterDelay(0.2f));
        }

        if (!inputLocked && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton1)))
        {
            PlaySelectSound();
            SelectButton(currentIndex);
        }
    }

    void UpdateSelection()
    {
        currentLeaf.SetActive(false);
        currentLeaf.transform.position = menuButtons[currentIndex].transform.position;
        currentLeaf.SetActive(true);

        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].SetSelected(i == currentIndex);
        }
    }

    IEnumerator UnlockInputAfterDelay(float delay)
    {
        inputLocked = true;
        yield return new WaitForSecondsRealtime(delay);
        inputLocked = false;
    }

    public void ActiverMenuMort()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);

        if (globalVolume != null)
            globalVolume.enabled = true;

        Time.timeScale = 0f;
        StartCoroutine(ShowMenuSequence());
    }

    private IEnumerator ShowMenuSequence()
    {
        yield return StartCoroutine(gameOverUI.GameOverSequence());

        foreach (GameObject button in menuButtons)
            button.SetActive(false);

        currentLeaf.SetActive(false);
        inputLocked = true;

        for (int i = 0; i < menuButtons.Length; i++)
        {
            GameObject button = menuButtons[i];
            button.SetActive(true);

            CanvasGroup group = button.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 0f;
                float duration = 0.3f;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    group.alpha = Mathf.Clamp01(elapsed / duration);
                    yield return null;
                }
                group.alpha = 1f;
            }
            yield return new WaitForSecondsRealtime(0.05f);
        }

        UpdateSelection();
        inputLocked = false;
    }

    public void SelectButton(int index)
    {
        switch (index)
        {
            case 0: RetryScene(); break;
            case 1: ReturnToMainMenu(); break;
            case 2: QuitGame(); break;
        }
    }

    public void RetryScene()
    {
        Time.timeScale = 1f;
        if (globalVolume != null)
            globalVolume.enabled = false;
        ObjectCollectable.ResetCollectables();
        SceneManager.LoadScene(sceneRetryName);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (globalVolume != null)
            globalVolume.enabled = false;
        ObjectCollectable.ResetCollectables();
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");
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
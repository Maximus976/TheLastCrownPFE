using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject[] menuButtons;
    public GameObject leafPrefab;

    private int currentIndex = 0;
    private bool isPaused = false;
    private bool inputLocked = false;

    private GameObject currentLeaf;
    private PauseMenuItem[] menuItems;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        currentLeaf = Instantiate(leafPrefab);
        currentLeaf.SetActive(false);

        menuItems = new PauseMenuItem[menuButtons.Length];
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuItems[i] = menuButtons[i].GetComponent<PauseMenuItem>();
            menuButtons[i].SetActive(false); // Important : cachés au départ
        }

        UpdateSelection();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            if (!isPaused)
                OpenPauseMenu();
            else
                ResumeGame();
        }

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
                UpdateSelection();

            StartCoroutine(UnlockInputAfterDelay(0.2f));
        }

        if (!inputLocked && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton1)))
        {
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

    public void OpenPauseMenu()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        StartCoroutine(ShowMenuButtons());
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }

    public void SelectButton(int index)
    {
        if (index == 0)
            ResumeGame();
        else if (index == 1)
            ReturnToMainMenu();
        else if (index == 2)
            QuitGame();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();
    }

    IEnumerator ShowMenuButtons()
    {
        foreach (GameObject button in menuButtons)
        {
            button.SetActive(false);
        }

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
                    float t = Mathf.Clamp01(elapsed / duration);
                    group.alpha = t;
                    yield return null;
                }

                group.alpha = 1f;
            }

            yield return new WaitForSecondsRealtime(0.05f); // délai entre chaque bouton
        }

        UpdateSelection();
        inputLocked = false;
    }
}
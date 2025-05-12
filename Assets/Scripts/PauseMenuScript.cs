using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject gameUI;
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

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton1))
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
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        gameUI.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        gameUI.SetActive(true);
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
}
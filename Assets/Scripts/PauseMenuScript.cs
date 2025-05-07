using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenuUI;   // Canvas du menu de pause
    public GameObject gameUI;        // UI du jeu � cacher quand le menu est activ�
    public GameObject[] menuButtons; // Boutons du menu (Reprendre, Menu principal, Quitter)
    public GameObject leafPrefab;    // Prefab du "leaf" (surbrillance du bouton s�lectionn�)

    private int currentIndex = 0;    // Index du bouton actuellement s�lectionn�
    private bool isPaused = false;   // Si le jeu est en pause ou non
    private bool inputLocked = false; // Pour �viter de naviguer trop vite

    private GameObject currentLeaf;  // Le "leaf" autour du bouton s�lectionn�
    private PauseMenuItem[] menuItems; // Pour g�rer les boutons avec surbrillance "leaf"

    void Start()
    {
        pauseMenuUI.SetActive(false);  // Le menu pause est d�sactiv� au d�part
        currentLeaf = Instantiate(leafPrefab); // Cr�e le leaf (surbrillance)
        currentLeaf.SetActive(false);  // Le cache au d�but

        menuItems = new PauseMenuItem[menuButtons.Length];
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuItems[i] = menuButtons[i].GetComponent<PauseMenuItem>(); // Associe chaque bouton au script
        }

        UpdateSelection(); // Initialisation de la s�lection par d�faut
    }

    void Update()
    {
        // Si le bouton de pause (JoystickButton9) est press�
        if (Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            if (!isPaused)
                OpenPauseMenu(); // Ouvre le menu pause
            else
                ResumeGame(); // Reprend le jeu
        }

        // Si le jeu est en pause, permet la navigation dans le menu
        if (!isPaused) return;

        // Navigation verticale avec le joystick
        float vertical = Input.GetAxisRaw("Vertical");
        if (!inputLocked && Mathf.Abs(vertical) > 0.5f)
        {
            int previousIndex = currentIndex;

            if (vertical < 0)
                currentIndex = (currentIndex + 1) % menuButtons.Length;  // Descend dans le menu
            else
                currentIndex = (currentIndex - 1 + menuButtons.Length) % menuButtons.Length;  // Monte dans le menu

            if (previousIndex != currentIndex)
                UpdateSelection();  // Met � jour la s�lection du bouton
            StartCoroutine(UnlockInputAfterDelay(0.2f));  // Verrouille l'entr�e bri�vement
        }

        // S�lectionner un bouton avec Enter ou JoystickButton1
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            SelectButton(currentIndex);  // Lance l'action du bouton s�lectionn�
        }
    }

    // Met � jour la s�lection des boutons visuellement (ajoute le "leaf")
    void UpdateSelection()
    {
        // Cache l'ancien leaf si un autre bouton est s�lectionn�
        currentLeaf.SetActive(false);

        // Met � jour le bouton s�lectionn� avec le "leaf"
        currentLeaf.transform.position = menuButtons[currentIndex].transform.position;
        currentLeaf.SetActive(true);

        // Met � jour la couleur du texte et l'�tat des leafs pour chaque bouton
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].SetSelected(i == currentIndex); // Applique la couleur et le "leaf" aux �l�ments
        }
    }

    // Emp�che la navigation trop rapide
    System.Collections.IEnumerator UnlockInputAfterDelay(float delay)
    {
        inputLocked = true;
        yield return new WaitForSecondsRealtime(delay);
        inputLocked = false;
    }

    // Ouvre le menu pause
    public void OpenPauseMenu()
    {
        isPaused = true;
        Time.timeScale = 0f;  // Met le jeu en pause
        pauseMenuUI.SetActive(true);
        gameUI.SetActive(false);  // Cache l'UI du jeu
    }

    // Reprend le jeu
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;  // Reprend le jeu
        pauseMenuUI.SetActive(false);  // Cache le menu pause
        gameUI.SetActive(true);  // R�affiche l'UI du jeu
    }

    // S�lectionne un bouton
    public void SelectButton(int index)
    {
        if (index == 0)
            ResumeGame();  // Reprendre le jeu
        else if (index == 1)
            ReturnToMainMenu();  // Retour au menu principal
        else if (index == 2)
            QuitGame();  // Quitter le jeu
    }

    // Retourne au menu principal
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;  // Reprend le jeu
        SceneManager.LoadScene("Menu"); // Charge la sc�ne du menu principal
    }

    // Quitter le jeu
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();  // Quitte l'application
    }
}
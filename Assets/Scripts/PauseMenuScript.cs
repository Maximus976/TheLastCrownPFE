using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenuUI;   // Canvas du menu de pause
    public GameObject gameUI;        // UI du jeu à cacher quand le menu est activé
    public GameObject[] menuButtons; // Boutons du menu (Reprendre, Menu principal, Quitter)
    public GameObject leafPrefab;    // Prefab du "leaf" (surbrillance du bouton sélectionné)

    private int currentIndex = 0;    // Index du bouton actuellement sélectionné
    private bool isPaused = false;   // Si le jeu est en pause ou non
    private bool inputLocked = false; // Pour éviter de naviguer trop vite

    private GameObject currentLeaf;  // Le "leaf" autour du bouton sélectionné
    private PauseMenuItem[] menuItems; // Pour gérer les boutons avec surbrillance "leaf"

    void Start()
    {
        pauseMenuUI.SetActive(false);  // Le menu pause est désactivé au départ
        currentLeaf = Instantiate(leafPrefab); // Crée le leaf (surbrillance)
        currentLeaf.SetActive(false);  // Le cache au début

        menuItems = new PauseMenuItem[menuButtons.Length];
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuItems[i] = menuButtons[i].GetComponent<PauseMenuItem>(); // Associe chaque bouton au script
        }

        UpdateSelection(); // Initialisation de la sélection par défaut
    }

    void Update()
    {
        // Si le bouton de pause (JoystickButton9) est pressé
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
                UpdateSelection();  // Met à jour la sélection du bouton
            StartCoroutine(UnlockInputAfterDelay(0.2f));  // Verrouille l'entrée brièvement
        }

        // Sélectionner un bouton avec Enter ou JoystickButton1
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            SelectButton(currentIndex);  // Lance l'action du bouton sélectionné
        }
    }

    // Met à jour la sélection des boutons visuellement (ajoute le "leaf")
    void UpdateSelection()
    {
        // Cache l'ancien leaf si un autre bouton est sélectionné
        currentLeaf.SetActive(false);

        // Met à jour le bouton sélectionné avec le "leaf"
        currentLeaf.transform.position = menuButtons[currentIndex].transform.position;
        currentLeaf.SetActive(true);

        // Met à jour la couleur du texte et l'état des leafs pour chaque bouton
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].SetSelected(i == currentIndex); // Applique la couleur et le "leaf" aux éléments
        }
    }

    // Empêche la navigation trop rapide
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
        gameUI.SetActive(true);  // Réaffiche l'UI du jeu
    }

    // Sélectionne un bouton
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
        SceneManager.LoadScene("Menu"); // Charge la scène du menu principal
    }

    // Quitter le jeu
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();  // Quitte l'application
    }
}
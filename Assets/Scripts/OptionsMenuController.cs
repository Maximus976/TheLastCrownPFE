using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class OptionsMenuController : MonoBehaviour
{
    public GameObject optionsMenu;        // Menu des options
    public GameObject mainMenu;           // Menu principal
    public Slider volumeSlider;           // Slider de volume
    public TMP_Text volumeText;           // Texte TMP pour le volume
    public EventSystem eventSystem;       // EventSystem de la scène

    void OnEnable()
    {
        // Juste lire et afficher la valeur du slider, pas la modifier
        volumeSlider.value = AudioListener.volume;
        UpdateVolumeText(volumeSlider.value);  // Utilisation de la méthode UpdateVolumeText
        eventSystem.SetSelectedGameObject(volumeSlider.gameObject);
    }

    void Update()
    {
        // Quitter le menu des options avec le bouton "O" (JoystickButton2)
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            BackToMainMenu();
        }
    }

    // Mise à jour du volume (appelé depuis le Slider)
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        UpdateVolumeText(volume);  // Mise à jour du texte lorsque le volume change
    }

    private void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            // Affiche "Volume XX%"
            volumeText.text = "Volume " + (volume * 100f).ToString("F0") + "%";
        }
    }

    public void BackToMainMenu()
    {
        if (mainMenu != null) mainMenu.SetActive(true);
        if (optionsMenu != null) optionsMenu.SetActive(false);

        // Remet le focus sur le menu principal
        var menuScript = mainMenu.GetComponent<MenuScript>();
        if (menuScript != null)
        {
            menuScript.enabled = true;
            menuScript.UpdateSelection();
        }
    }
}
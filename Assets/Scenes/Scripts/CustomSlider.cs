using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomSlider : MonoBehaviour
{
    public Slider slider;  // Le slider à personnaliser
    public Image[] sliderBlocks;  // Les images représentant les blocs
    public Color activeColor = Color.yellow;  // Couleur active du bloc
    public Color inactiveColor = Color.white;  // Couleur du bloc inactif

    public float joystickSpeed = 0.1f;  // Vitesse de défilement du slider avec le joystick
    public string horizontalAxis = "Horizontal";  // Nom de l'axe horizontal pour le joystick (par défaut "Horizontal")

    void Start()
    {
        // Initialisation du slider
        slider.onValueChanged.AddListener(UpdateSliderVisual);
        UpdateSliderVisual(slider.value);  // Initialisation au démarrage
    }

    void Update()
    {
        // Contrôler le slider avec le joystick (généralement l'axe horizontal)
        float joystickInput = Input.GetAxis(horizontalAxis);  // Valeur entre -1 et 1 pour l'axe horizontal
        slider.value += joystickInput * joystickSpeed;  // Ajuster la valeur du slider en fonction du mouvement du joystick

        // Limiter la valeur du slider entre 0 et 1
        slider.value = Mathf.Clamp(slider.value, 0f, 1f);

        // Mettre à jour l'affichage des blocs en fonction de la nouvelle valeur
        UpdateSliderVisual(slider.value);
    }

    // Fonction pour mettre à jour les visuels du slider
    void UpdateSliderVisual(float value)
    {
        int activeBlocks = Mathf.FloorToInt(value * sliderBlocks.Length);
        for (int i = 0; i < sliderBlocks.Length; i++)
        {
            // Modifier la couleur des blocs selon la valeur du slider
            if (i < activeBlocks)
                sliderBlocks[i].color = activeColor;  // Bloc actif
            else
                sliderBlocks[i].color = inactiveColor;  // Bloc inactif
        }
    }
}
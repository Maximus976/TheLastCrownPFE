using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomSlider : MonoBehaviour
{
    public Slider slider;  // Le slider � personnaliser
    public Image[] sliderBlocks;  // Les images repr�sentant les blocs
    public Color activeColor = Color.yellow;  // Couleur active du bloc
    public Color inactiveColor = Color.white;  // Couleur du bloc inactif

    public float joystickSpeed = 0.1f;  // Vitesse de d�filement du slider avec le joystick
    public string horizontalAxis = "Horizontal";  // Nom de l'axe horizontal pour le joystick (par d�faut "Horizontal")

    void Start()
    {
        // Initialisation du slider
        slider.onValueChanged.AddListener(UpdateSliderVisual);
        UpdateSliderVisual(slider.value);  // Initialisation au d�marrage
    }

    void Update()
    {
        // Contr�ler le slider avec le joystick (g�n�ralement l'axe horizontal)
        float joystickInput = Input.GetAxis(horizontalAxis);  // Valeur entre -1 et 1 pour l'axe horizontal
        slider.value += joystickInput * joystickSpeed;  // Ajuster la valeur du slider en fonction du mouvement du joystick

        // Limiter la valeur du slider entre 0 et 1
        slider.value = Mathf.Clamp(slider.value, 0f, 1f);

        // Mettre � jour l'affichage des blocs en fonction de la nouvelle valeur
        UpdateSliderVisual(slider.value);
    }

    // Fonction pour mettre � jour les visuels du slider
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
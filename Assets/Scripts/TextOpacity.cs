using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextOpacity : MonoBehaviour
{
    [Header("UI Elements")]
    public Text targetText;           // Texte � faire appara�tre
    public Image targetImage;         // Image � faire appara�tre
    public float textFadeDuration = 2f; // Dur�e de l'effet de fondu pour le texte
    public float imageFadeDelay = 1f;  // D�lai avant le d�but du fondu de l'image
    public float imageFadeDuration = 2f; // Dur�e de l'effet de fondu pour l'image

    private float textFadeTimer = 0f;  // Chronom�tre pour le fondu du texte
    private float imageFadeTimer = 0f; // Chronom�tre pour le fondu de l'image
    private Color initialTextColor;    // Couleur initiale du texte
    private Color initialImageColor;   // Couleur initiale de l'image

    void Start()
    {
        // V�rification et initialisation du texte
        if (targetText != null)
        {
            initialTextColor = targetText.color;
            targetText.color = new Color(initialTextColor.r, initialTextColor.g, initialTextColor.b, 0f);
        }
        else
        {
            Debug.LogError("Aucun texte assign� au script !");
        }

        // V�rification et initialisation de l'image
        if (targetImage != null)
        {
            initialImageColor = targetImage.color;
            targetImage.color = new Color(initialImageColor.r, initialImageColor.g, initialImageColor.b, 0f);
        }
        else
        {
            Debug.LogError("Aucune image assign�e au script !");
        }
    }

    void Update()
    {
        // Gestion du fondu pour le texte
        if (textFadeTimer < textFadeDuration)
        {
            textFadeTimer += Time.deltaTime;

            // Calcul de la nouvelle opacit� (de 0 � 1)
            float alpha = Mathf.Clamp01(textFadeTimer / textFadeDuration);

            // Mise � jour de la couleur du texte
            if (targetText != null)
            {
                targetText.color = new Color(initialTextColor.r, initialTextColor.g, initialTextColor.b, alpha);
            }
        }

        // Gestion du fondu pour l'image avec un d�lai
        if (textFadeTimer >= imageFadeDelay && imageFadeTimer < imageFadeDuration)
        {
            imageFadeTimer += Time.deltaTime;

            // Calcul de la nouvelle opacit� (de 0 � 1)
            float alpha = Mathf.Clamp01(imageFadeTimer / imageFadeDuration);

            // Mise � jour de la couleur de l'image
            if (targetImage != null)
            {
                targetImage.color = new Color(initialImageColor.r, initialImageColor.g, initialImageColor.b, alpha);
            }
        }
    }
}
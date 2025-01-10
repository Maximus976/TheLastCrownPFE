using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextOpacity : MonoBehaviour
{
    public Text targetText;      // Le texte � faire appara�tre
    public float fadeDuration = 2f;  // Dur�e de l'effet de fondu

    private float fadeTimer = 0f;    // Chronom�tre pour le fondu
    private Color initialColor;     // Couleur initiale du texte

    void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("Aucun texte assign� au script !");
            return;
        }

        // R�cup�rer la couleur initiale du texte (et mettre l'opacit� � 0)
        initialColor = targetText.color;
        targetText.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
    }

    void Update()
    {
        // Si l'effet de fondu n'est pas termin�
        if (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;

            // Calcul de la nouvelle opacit� (de 0 � 1)
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

            // Mise � jour de la couleur du texte
            targetText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
        }
    }
}

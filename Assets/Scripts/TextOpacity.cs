using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextOpacity : MonoBehaviour
{
    public Text targetText;      // Le texte à faire apparaître
    public float fadeDuration = 2f;  // Durée de l'effet de fondu

    private float fadeTimer = 0f;    // Chronomètre pour le fondu
    private Color initialColor;     // Couleur initiale du texte

    void Start()
    {
        if (targetText == null)
        {
            Debug.LogError("Aucun texte assigné au script !");
            return;
        }

        // Récupérer la couleur initiale du texte (et mettre l'opacité à 0)
        initialColor = targetText.color;
        targetText.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
    }

    void Update()
    {
        // Si l'effet de fondu n'est pas terminé
        if (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;

            // Calcul de la nouvelle opacité (de 0 à 1)
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

            // Mise à jour de la couleur du texte
            targetText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
        }
    }
}

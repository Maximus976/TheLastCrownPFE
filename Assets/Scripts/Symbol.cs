using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    private Renderer symbolRenderer;  // Référence au Renderer du symbole
    private Material symbolMaterial;  // Référence au matériau du symbole
    public Color glowColor = Color.blue;  // La couleur d'émission (bleu)
    public float emissionIntensity = 1.0f;  // L'intensité de l'émission

    void Start()
    {
        // Récupère le Renderer et le matériau au démarrage
        symbolRenderer = GetComponent<Renderer>();
        if (symbolRenderer != null)
        {
            symbolMaterial = symbolRenderer.material;
        }
        else
        {
            Debug.LogError("Le Renderer n'a pas été trouvé sur l'objet " + gameObject.name);
        }
    }

    public void SetGlow(bool state)
    {
        if (symbolMaterial != null)
        {
            if (state)
            {
                // Active l'émission avec la couleur spécifiée
                symbolMaterial.SetColor("_EmissionColor", glowColor * emissionIntensity);  // Modifie la couleur émissive
                symbolRenderer.material.EnableKeyword("_EMISSION");  // Active l'effet émissif
            }
            else
            {
                // Désactive l'émission
                symbolMaterial.SetColor("_EmissionColor", Color.black);  // Désactive l'émission en la mettant à noir
                symbolRenderer.material.DisableKeyword("_EMISSION");  // Désactive l'effet émissif
            }
        }
    }
}
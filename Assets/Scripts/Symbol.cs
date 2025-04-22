using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    private Renderer symbolRenderer;  // R�f�rence au Renderer du symbole
    private Material symbolMaterial;  // R�f�rence au mat�riau du symbole
    public Color glowColor = Color.blue;  // La couleur d'�mission (bleu)
    public float emissionIntensity = 1.0f;  // L'intensit� de l'�mission

    void Start()
    {
        // R�cup�re le Renderer et le mat�riau au d�marrage
        symbolRenderer = GetComponent<Renderer>();
        if (symbolRenderer != null)
        {
            symbolMaterial = symbolRenderer.material;
        }
        else
        {
            Debug.LogError("Le Renderer n'a pas �t� trouv� sur l'objet " + gameObject.name);
        }
    }

    public void SetGlow(bool state)
    {
        if (symbolMaterial != null)
        {
            if (state)
            {
                // Active l'�mission avec la couleur sp�cifi�e
                symbolMaterial.SetColor("_EmissionColor", glowColor * emissionIntensity);  // Modifie la couleur �missive
                symbolRenderer.material.EnableKeyword("_EMISSION");  // Active l'effet �missif
            }
            else
            {
                // D�sactive l'�mission
                symbolMaterial.SetColor("_EmissionColor", Color.black);  // D�sactive l'�mission en la mettant � noir
                symbolRenderer.material.DisableKeyword("_EMISSION");  // D�sactive l'effet �missif
            }
        }
    }
}
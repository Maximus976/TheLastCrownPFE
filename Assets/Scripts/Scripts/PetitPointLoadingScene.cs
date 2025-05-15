using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PetitPointLoadingScene : MonoBehaviour
{
    public float waveSpeed = 2f;    // Vitesse du rebond
    public float waveHeight = 10f;   // Hauteur du rebond
    public float delayBetweenDots = 0.5f; // Délai entre les débuts des animations des points

    private TMP_Text tmpText;
    private TMP_TextInfo textInfo;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;
    }

    void Update()
    {
        tmpText.ForceMeshUpdate(); // Met à jour le texte pour pouvoir modifier les vertices

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            var verts = textInfo.meshInfo[0].vertices;
            int index = textInfo.characterInfo[i].vertexIndex;

            // Décalage pour chaque point
            float delay = i * delayBetweenDots;

            // L'animation du rebond commence après un délai basé sur l'index du point
            float offsetY = Mathf.Sin((Time.time - delay) * waveSpeed) * waveHeight;

            verts[index + 0].y += offsetY;
            verts[index + 1].y += offsetY;
            verts[index + 2].y += offsetY;
            verts[index + 3].y += offsetY;
        }

        tmpText.UpdateVertexData(); // Applique les changements de position aux vertices
    }
}
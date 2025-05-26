using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class pilliiermanager : MonoBehaviour
{

    public Pillar[] pillars;
    public GameObject[] symbolsToAffect;
    public Animator doorAnimator;
    public string openDoorTriggerName = "Open";
    public GameObject[] symbolsToActivate;
    public AudioSource doorOpenSound; // ?? Ajout de l'AudioSource pour le son de la porte

    private bool isMoving = false;
    public bool IsPuzzleSolved { get; private set; } = false;

    public void TogglePillars(int[] indicesToToggle)
    {
        if (isMoving || IsPuzzleSolved)
            return;

        StartCoroutine(TogglePillarsRoutine(indicesToToggle));
    }

    private IEnumerator TogglePillarsRoutine(int[] indicesToToggle)
    {
        isMoving = true;

        foreach (int i in indicesToToggle)
        {
            if (i >= 0 && i < pillars.Length)
            {
                if (pillars[i].IsActivated())
                    pillars[i].Deactivate();
                else
                    pillars[i].Activate();
            }
        }

        yield return new WaitUntil(() =>
        {
            foreach (var p in pillars)
                if (p.IsMoving)
                    return false;
            return true;
        });

        isMoving = false;

        if (AreAllPillarsUp())
        {
            IsPuzzleSolved = true;
            OnPuzzleSolved();
        }
    }

    private bool AreAllPillarsUp()
    {
        foreach (var p in pillars)
            if (!p.IsActivated())
                return false;
        return true;
    }

    private void OnPuzzleSolved()
    {
        // Active les symboles
        Color emissionColor;
        if (!ColorUtility.TryParseHtmlString("#00F7FF", out emissionColor))
        {
            Debug.LogWarning("Impossible de parser la couleur hex.");
            emissionColor = Color.cyan; // fallback
        }

        foreach (var sym in symbolsToActivate)
        {
            Renderer renderer = sym.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = renderer.material;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor * 2f);
            }
        }

        // Déclenche l'animation de la porte
        if (doorAnimator != null)
            doorAnimator.SetTrigger(openDoorTriggerName);

        // ?? Joue le son d’ouverture de porte
        if (doorOpenSound != null)
            doorOpenSound.Play();
    }
}
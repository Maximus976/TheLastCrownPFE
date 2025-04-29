using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PillierAnim : MonoBehaviour
{
    public bool[] pillarStates = new bool[3] { false, false, false };  // Initialement tous les piliers en bas
    public GameObject[] pillarObjects;
    public GameObject[] glowSymbols;

    public float moveDuration = 2f;

    public AudioSource liftSound;     // Son quand le pilier se l�ve ou descend
    public AudioSource shakeSound;    // Son de secousse
    public AudioSource victorySound;

    private bool hasPlayedVictorySound = false;
    private int pillarsMoving = 0;
    public bool isPuzzleSolved = false;  // Nouvelle variable pour savoir si l'�nigme est r�solue

    void Start()
    {
        // Initialisation des positions de base des piliers
        Vector3[] basePositions = new Vector3[pillarObjects.Length];
        for (int i = 0; i < pillarObjects.Length; i++)
        {
            basePositions[i] = pillarObjects[i].transform.localPosition;
        }
    }

    // M�thode pour v�rifier si un pilier est en mouvement
    public bool IsAnyPillarMoving()
    {
        return pillarsMoving > 0;
    }

    // Toggle des piliers
    public void TogglePillars(int[] indices)
    {
        if (isPuzzleSolved) return;  // Si l'�nigme est r�solue, on ne permet pas de manipuler les piliers

        foreach (int i in indices)
        {
            pillarStates[i] = !pillarStates[i];  // Bascule l'�tat du pilier
            StartCoroutine(MovePillar(i));  // Lance l'animation de d�placement du pilier
        }
    }

    private IEnumerator MovePillar(int index)
    {
        pillarsMoving++;

        UpdateLiftSoundVolume(); // <-- On ajuste le volume d�s qu�un pilier commence � bouger

        Vector3 startPos = pillarObjects[index].transform.localPosition;
        Vector3 targetPos = pillarStates[index]
            ? new Vector3(startPos.x, -12.69f, startPos.z)  // Pilier lev�
            : new Vector3(startPos.x, -16.07f, startPos.z);  // Pilier baiss�

        float elapsedTime = 0f;

        if (liftSound != null && !liftSound.isPlaying)
        {
            liftSound.Play();
        }

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            pillarObjects[index].transform.localPosition = Vector3.Lerp(startPos, targetPos, smoothT);

            yield return null;
        }

        pillarObjects[index].transform.localPosition = targetPos;

        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ShakePillar(index));

        pillarsMoving--;

        UpdateLiftSoundVolume(); // <-- On met � jour le volume � la fin du mouvement

        if (pillarsMoving == 0 && liftSound != null)
        {
            liftSound.Stop();
        }

        if (pillarsMoving == 0)
            CheckWinCondition();
    }

    // Nouvelle m�thode pour ajuster dynamiquement le volume du liftSound
    private void UpdateLiftSoundVolume()
    {
        if (liftSound == null) return;

        // Volume de base 1.0, on ajoute 0.2 par pilier en mouvement (max 2.0 par exemple)
        float volume = Mathf.Clamp(1f + 0.2f * (pillarsMoving - 1), 0f, 2f);
        liftSound.volume = volume;
    }

    private IEnumerator ShakePillar(int index)
    {
        Vector3 originalPosition = pillarObjects[index].transform.localPosition;

        if (shakeSound != null)
            shakeSound.Play();

        for (int i = 0; i < 5; i++)
        {
            float shakeAmount = 0.05f;
            Vector3 shakePosition = originalPosition + new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount)
            );

            pillarObjects[index].transform.localPosition = shakePosition;
            yield return new WaitForSeconds(0.05f);
        }

        pillarObjects[index].transform.localPosition = originalPosition;
    }

    private void CheckWinCondition()
    {
        if (pillarStates[0] && pillarStates[1] && pillarStates[2])
        {
            Debug.Log("�nigme r�solue !");

            foreach (var symbol in glowSymbols)
            {
                Symbol symbolComponent = symbol.GetComponent<Symbol>();
                if (symbolComponent != null)
                    symbolComponent.SetGlow(true);
            }

            if (!hasPlayedVictorySound && victorySound != null)
            {
                victorySound.Play();
                hasPlayedVictorySound = true;
            }

            // D�sactive le puzzle une fois r�solu
            isPuzzleSolved = true;

            // Appelez la destruction du mur ici
            DestroyWall();  // Appel de la fonction pour d�truire le mur
        }
        else
        {
            foreach (var symbol in glowSymbols)
            {
                Symbol symbolComponent = symbol.GetComponent<Symbol>();
                if (symbolComponent != null)
                    symbolComponent.SetGlow(false);
            }

            hasPlayedVictorySound = false;
        }
    }

    private void DestroyWall()
    {
        GameObject mur = GameObject.FindWithTag("MurDestructible");
        if (mur != null)
        {
            MurDestructible destructible = mur.GetComponent<MurDestructible>();
            if (destructible != null)
                destructible.DetruireMur();  // Appel de la m�thode pour d�truire le mur
        }
    }
    // Nouvelle m�thode pour r�initialiser le puzzle
    public void ResetPuzzle()
    {
        if (!isPuzzleSolved) return;  // Si l'�nigme n'est pas encore r�solue, on ne fait rien

        // R�initialisation de l'�tat des piliers et de l'�nigme
        for (int i = 0; i < pillarStates.Length; i++)
        {
            pillarStates[i] = false;  // Tous les piliers sont en bas
            pillarObjects[i].transform.localPosition = new Vector3(pillarObjects[i].transform.localPosition.x, -16.07f, pillarObjects[i].transform.localPosition.z);
        }

        // R�initialisation des symboles lumineux
        foreach (var symbol in glowSymbols)
        {
            Symbol symbolComponent = symbol.GetComponent<Symbol>();
            if (symbolComponent != null)
                symbolComponent.SetGlow(false);
        }

        isPuzzleSolved = false;  // R�initialisation de la condition de victoire
        hasPlayedVictorySound = false;  // R�initialisation du son de victoire
    }
}
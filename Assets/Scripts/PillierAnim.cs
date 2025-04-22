using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PillierAnim : MonoBehaviour
{
    public bool[] pillarStates = new bool[3] { false, false, true };
    public GameObject[] pillarObjects;
    public GameObject[] glowSymbols;

    public float moveDuration = 2f;

    public AudioSource liftSound;     // Son quand le pilier se lève ou descend
    public AudioSource shakeSound;    // Son de secousse
    public AudioSource victorySound;

    public cameracutscene cameraCutscene;

    private Vector3[] basePositions;
    private bool hasPlayedVictorySound = false;
    private int pillarsMoving = 0;

    void Start()
    {
        basePositions = new Vector3[pillarObjects.Length];
        for (int i = 0; i < pillarObjects.Length; i++)
        {
            basePositions[i] = pillarObjects[i].transform.localPosition;
        }
    }

    public void TogglePillars(int[] indices)
    {
        foreach (int i in indices)
        {
            pillarStates[i] = !pillarStates[i];
            StartCoroutine(MovePillar(i));
        }
    }

    private IEnumerator MovePillar(int index)
    {
        pillarsMoving++;

        Vector3 startPos = pillarObjects[index].transform.localPosition;
        Vector3 targetPos = pillarStates[index]
            ? new Vector3(startPos.x, -12.69f, startPos.z)
            : new Vector3(startPos.x, -16.07f, startPos.z);

        float elapsedTime = 0f;

        if (liftSound != null)
        {
            liftSound.volume = 1f;
            liftSound.Play();
        }

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            pillarObjects[index].transform.localPosition = Vector3.Lerp(startPos, targetPos, smoothT);

            // Fondu progressif du son vers la fin
            if (liftSound != null && elapsedTime > moveDuration * 0.7f)
            {
                liftSound.volume = Mathf.Lerp(1f, 0f, (elapsedTime - moveDuration * 0.7f) / (moveDuration * 0.3f));
            }

            yield return null;
        }

        if (liftSound != null)
            liftSound.Stop();

        pillarObjects[index].transform.localPosition = targetPos;

        // Laisse un petit temps pour que le son de levage disparaisse vraiment
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(ShakePillar(index));

        pillarsMoving--;

        if (pillarsMoving == 0)
            CheckWinCondition();
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
            Debug.Log("Énigme résolue !");

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

            if (cameraCutscene != null)
            {
                Debug.Log("Cinématique appelée !");
                cameraCutscene.PlayCutscene();
            }
            else
            {
                Debug.LogWarning("CameraCutscene est NULL !");
            }

            GameObject mur = GameObject.FindWithTag("MurDestructible");
            if (mur != null)
            {
                MurDestructible destructible = mur.GetComponent<MurDestructible>();
                if (destructible != null)
                    destructible.DetruireMur();
            }
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
}
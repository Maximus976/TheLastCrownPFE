using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public int pillarIndex;

    private bool isActivated = false;
    private bool isLocked = false;
    private bool isMoving = false;

    public float moveHeight = 3.3f;
    public float moveDuration = 1f;

    public AudioSource moveSound;    // Son quand le pilier monte ou descend
    public AudioSource shakeSound;   // Son quand le pilier shake

    public float shakeDuration = 0.5f;
    public float shakeAmount = 0.1f;

    private Vector3 basePosition;

    private void Start()
    {
        basePosition = transform.position;
    }

    public bool IsActivated()
    {
        return isActivated;
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    public bool IsMoving  // Propriété au lieu de méthode
    {
        get { return isMoving; }
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Activate()
    {
        if (isLocked || isActivated || isMoving)
            return;

        isActivated = true;
        StartCoroutine(MovePillar(basePosition + Vector3.up * moveHeight));
    }

    public void Deactivate()
    {
        if (isLocked || !isActivated || isMoving)
            return;

        isActivated = false;
        StartCoroutine(MovePillar(basePosition));
    }

    private IEnumerator MovePillar(Vector3 targetPosition)
    {
        isMoving = true;

        if (moveSound)
            moveSound.Play();

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        // Après le mouvement, faire un petit shake
        yield return Shake();

        isMoving = false;
    }

    private IEnumerator Shake()
    {
        if (shakeSound)
            shakeSound.Play();

        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float z = Random.Range(-shakeAmount, shakeAmount);
            transform.position = originalPos + new Vector3(x, 0, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
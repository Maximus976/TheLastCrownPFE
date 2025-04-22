using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPos;
    private bool isShaking = false;

    public float shakeDuration = 0f;
    public float shakeMagnitude = 0.1f;  // L�g�re secousse
    public float shakeSmoothness = 0.1f; // Fluidit� du tremblement

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        if (isShaking)
        {
            if (shakeDuration > 0)
            {
                // Applique un l�ger d�placement autour de la position actuelle
                Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                shakeOffset.z = 0;  // Limite le tremblement � deux dimensions (X et Y), pas en profondeur

                // Applique l'effet de tremblement � la position actuelle
                transform.localPosition = originalPos + shakeOffset;

                // R�duit la dur�e du tremblement
                shakeDuration -= Time.deltaTime;
            }
            else
            {
                // R�initialisation de la position � l'�tat normal apr�s la secousse
                isShaking = false;
                transform.localPosition = originalPos;
            }
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        isShaking = true;
    }
}
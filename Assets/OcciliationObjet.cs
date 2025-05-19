using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcciliationObjet : MonoBehaviour
{
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1f;
    public float rotationSpeed = 30f;

    private Vector3 startPosition;
    private bool isFloating = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (!isFloating) return;

        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPosition + new Vector3(0f, offsetY, 0f);
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    public void DisableFloating()
    {
        isFloating = false;
    }

    public void EnableFloating()
    {
        isFloating = true;
        startPosition = transform.position;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MannequinVie : MonoBehaviour
{
    public Slider healthBar; // R�f�rence au Slider de la barre de vie
    private Mannequin mannequin; // R�f�rence au script du mannequin

    void Start()
    {
        mannequin = GetComponent<Mannequin>();
        if (mannequin != null && healthBar != null)
        {
            healthBar.maxValue = mannequin.maxHealth;
            healthBar.value = mannequin.maxHealth;
        }
    }

    void Update()
    {
        if (mannequin != null && healthBar != null)
        {
            healthBar.value = mannequin.currentHealth;
            healthBar.transform.LookAt(Camera.main.transform); // Faire face � la cam�ra
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Destructible : MonoBehaviour
{
    [Header("Paramètres de l'objet")]
    public int health = 100;

    [Header("Effets visuels")]
    public GameObject destructionEffect;
    public GameObject destructionEffect2;
    public GameObject destructionEffect3;
    public GameObject impactEffect;

    [Header("Effets sonores")]
    public AudioClip impactSound;
    public AudioClip destroySound;
    public float soundVolume = 1f;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {health}");

        // Effet d'impact visuel
        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(impact, 2f);
        }

        // Son d'impact
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, transform.position, soundVolume);
        }

        if (health <= 0)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        if (destructionEffect != null)
        {
            // Effet de destruction principal
            GameObject effect1 = Instantiate(destructionEffect, transform.position, transform.rotation);

            // ?? Jouer le son exactement quand l’effet apparaît
            if (destroySound != null)
            {
                AudioSource.PlayClipAtPoint(destroySound, transform.position, soundVolume);
            }

            if (destructionEffect2 != null)
                Instantiate(destructionEffect2, transform.position, transform.rotation);

            if (destructionEffect3 != null)
                Instantiate(destructionEffect3, transform.position, transform.rotation);

            Destroy(effect1, 5f);
        }

        Debug.Log($"{gameObject.name} destroyed!");
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Destructible : MonoBehaviour
{
    [Header("Paramètres de l'objet")]
    public int health = 100;
    public GameObject destructionEffect;
    public GameObject destructionEffect2;
    public GameObject destructionEffect3; // Effet d'étoiles de soin
    public GameObject impactEffect;       // ?? Effet d'impact à l'impact

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {health}");

        // ?? Instancier l'effet d'impact à la position actuelle
        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(impact, 2f); // Durée de vie de l'effet d'impact
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
            GameObject effect1 = Instantiate(destructionEffect, transform.position, transform.rotation);
            Instantiate(destructionEffect2, transform.position, transform.rotation);

            if (destructionEffect3 != null)
            {
                Instantiate(destructionEffect3, transform.position, transform.rotation);
            }

            Destroy(effect1, 5f);
        }

        Debug.Log($"{gameObject.name} destroyed!");
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAutoDestroy : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private string[] ignoreTags = { "Ennemi" };

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (string tag in ignoreTags)
        {
            if (other.CompareTag(tag)) return;
        }
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}

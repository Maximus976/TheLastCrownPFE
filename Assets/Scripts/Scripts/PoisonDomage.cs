using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDomage : MonoBehaviour
{
    public int poisonDamage = 5;
    public float damageInterval = 1f; // toutes les 1s
    private Dictionary<GameObject, float> lastDamageTime = new Dictionary<GameObject, float>();

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!lastDamageTime.ContainsKey(other.gameObject))
                lastDamageTime[other.gameObject] = 0f;

            if (Time.time - lastDamageTime[other.gameObject] >= damageInterval)
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(poisonDamage);
                    lastDamageTime[other.gameObject] = Time.time;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Supprime l'entrée quand le joueur quitte le nuage
        if (lastDamageTime.ContainsKey(other.gameObject))
            lastDamageTime.Remove(other.gameObject);
    }
}

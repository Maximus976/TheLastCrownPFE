using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int damageAmount = 40;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Ennemi"))
        {
            Debug.Log("Test Script Sword Domage _ Tag");
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }
}

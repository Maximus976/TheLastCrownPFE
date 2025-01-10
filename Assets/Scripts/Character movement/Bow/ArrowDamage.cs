using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public int damage = 40; // Dégâts infligés par la flèche
    public float lifetime = 5f; // Temps avant que la flèche ne soit détruite

    private void Start()
    {
        // Détruire la flèche après un certain temps pour éviter qu'elle reste inutilement dans la scène
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifie si la flèche touche un ennemi
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Inflige des dégâts à l'ennemi
            enemyHealth.TakeDamage(damage);

            // Détruit la flèche après l'impact
            Destroy(gameObject);
        }
    }
}

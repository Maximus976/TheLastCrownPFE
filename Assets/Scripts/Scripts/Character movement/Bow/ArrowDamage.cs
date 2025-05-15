using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public int damage = 40; // D�g�ts inflig�s par la fl�che
    public float lifetime = 5f; // Temps avant que la fl�che ne soit d�truite

    private void Start()
    {
        // D�truire la fl�che apr�s un certain temps pour �viter qu'elle reste inutilement dans la sc�ne
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // V�rifie si la fl�che touche un ennemi
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Inflige des d�g�ts � l'ennemi
            enemyHealth.TakeDamage(damage);

            // D�truit la fl�che apr�s l'impact
            Destroy(gameObject);
        }
    }
}

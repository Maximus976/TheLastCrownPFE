using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed;
    private int damage;

    public void Initialize(Vector3 direction, float projectileSpeed, int projectileDamage)
    {
        moveDirection = direction.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;

        Destroy(gameObject, 5f); // Auto-destruction si rien touché
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}

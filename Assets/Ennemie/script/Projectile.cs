using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private int damage;

    public void Initialize(Vector3 target, float projectileSpeed, int projectileDamage)
    {
        targetPosition = target;
        speed = projectileSpeed;
        damage = projectileDamage;
        Destroy(gameObject, 5f); // auto-destroy après 5s
    }

    private void Update()
    {
        Vector3 dir = (targetPosition - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

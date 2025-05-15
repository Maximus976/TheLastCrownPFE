using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    private Health health;
    private EnemyMovement movement;

    void Start()
    {
        health = GetComponent<Health>();
        movement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if (health != null && movement != null && healthBarDead())
        {
            movement.Die();
            Destroy(this); // évite que ça se déclenche à chaque frame
        }
    }

    bool healthBarDead()
    {
        // check de la vie en "lecture seule"
        System.Reflection.FieldInfo field = typeof(Health).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            int hp = (int)field.GetValue(health);
            return hp <= 0;
        }
        return false;
    }
}

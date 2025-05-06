using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAtk : MonoBehaviour
{
    public int damage = 25;
    private bool canDealDamage = false;

    public void EnableDamage()
    {
        canDealDamage = true;
    }

    public void DisableDamage()
    {
        canDealDamage = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        var destructible = other.GetComponent<Destructible>();
        if (destructible != null)
        {
            destructible.TakeDamage(damage);
        }
    }
}
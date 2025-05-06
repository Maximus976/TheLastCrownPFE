using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Destructible : MonoBehaviour
{
    [Header("Param�tres de l'objet")]
    public int health = 100; 
    public int damageOnClick = 25; 
    public GameObject destructionEffect;
    public GameObject destructionEffect2;


  

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("D�g�ts re�us : " + damage + " PV restants : " + health);

        if (health <= 0)
        {
            DestroyObject();
        }
    }

    
    private void DestroyObject()
    {
        GameObject effect1 = null;
      
        if (destructionEffect != null)
        {
            
           effect1 =Instantiate(destructionEffect, transform.position, transform.rotation);
            Instantiate(destructionEffect2, transform.position, transform.rotation);
        }
        if (effect1 != null) Destroy(effect1, 5f);
     
        

        Destroy(gameObject);
        Debug.Log("Objet d�truit !");
        StartCoroutine(DestroyAfterDelay(2f));
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        Debug.Log("Objet compl�tement d�truit !");
    }
    private void OnMouseDown()
    {
        TakeDamage(damageOnClick);
    }

}
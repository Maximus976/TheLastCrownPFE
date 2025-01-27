using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fight : MonoBehaviour
{
    public Animation animationComponent; // Référence au Legacy Animation Component
    public AnimationClip attackClip; // Clip d'animation d'attaque
    public float cooldown = 1.0f; // Temps entre deux attaques
    private float nextAttackTime = 0f; // Temps de la prochaine attaque autorisée

    void Start()
    {

        if (animationComponent == null)
        {
            animationComponent = GetComponent<Animation>();

            if (animationComponent == null)
            {
                Debug.LogError("No Animation Component found! Add one to use this script.");
            }
        }

        if (attackClip != null)
        {
            // Ajouter le clip au composant Animation
            animationComponent.AddClip(attackClip, "Attack");
        }
    }

    void Update()
    {
        // Si l'utilisateur clique et que le cooldown est respecté
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            PlayAttackAnimation();
        }
    }

    void PlayAttackAnimation()
    {
        if (attackClip != null && animationComponent != null)
        {
            animationComponent.Play("Attack"); // Joue l'animation d'attaque
            nextAttackTime = Time.time + cooldown; // Met à jour le temps pour la prochaine attaque
            Debug.Log("Attack animation played.");
        }
        else
        {
            Debug.LogError("Attack animation clip or Animation Component is missing!");
        }
    }
}



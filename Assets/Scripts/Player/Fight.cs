using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fight : MonoBehaviour
{
    public Animation animationComponent;
    public AnimationClip attackClip;
    public float cooldown = 1.0f;
    private float nextAttackTime = 0f;

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
            animationComponent.AddClip(attackClip, "Attack");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            PlayAttackAnimation();
        }
    }

    void PlayAttackAnimation()
    {
        if (attackClip != null && animationComponent != null)
        {
            animationComponent.Play("Attack");
            nextAttackTime = Time.time + cooldown;
            Debug.Log("Attack animation played.");
        }
    }
}



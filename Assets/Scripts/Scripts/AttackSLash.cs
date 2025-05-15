using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSLash : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");  // Lance l'animation
        }
        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<Animation>().Play("Attack");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(5, -5, -5);
    public float followSpeed = 10f;

    void Start()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;

            // Utilisation de MoveTowards pour un suivi plus direct et fluide
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    /*void LateUpdate()
    {
        
    }*/
}

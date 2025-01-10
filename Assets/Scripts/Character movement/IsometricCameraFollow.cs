using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -15);
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the target position based on the player's position and the offset.
            Vector3 targetPosition = target.position + offset;

            // Smoothly move the camera towards the target position.
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}

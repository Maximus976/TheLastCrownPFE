using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // R�f�rence du joueur
    public Vector3 offset = new Vector3(0, 15, -15); // D�calage de la cam�ra
    public float smoothSpeed = 5f; // Vitesse de transition

    void LateUpdate()
    {
        if (player != null)
        {
            // Position cible de la cam�ra
            Vector3 targetPosition = player.position + offset;

            // Lerp pour un suivi fluide
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}


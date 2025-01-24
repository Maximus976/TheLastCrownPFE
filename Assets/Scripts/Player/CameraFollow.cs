using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Référence du joueur
    public Vector3 offset = new Vector3(0, 15, -15); // Décalage de la caméra
    public float smoothSpeed = 5f; // Vitesse de transition

    void LateUpdate()
    {
        if (player != null)
        {
            // Position cible de la caméra
            Vector3 targetPosition = player.position + offset;

            // Lerp pour un suivi fluide
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}


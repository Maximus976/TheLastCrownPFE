using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // Préfab de la flèche
    [SerializeField] private Transform arrowSpawnPoint; // Point d'apparition des flèches
    [SerializeField] private float arrowSpeed = 20f; // Vitesse de la flèche
    [SerializeField] private float fireRate = 1f; // Temps minimum entre deux tirs
    [SerializeField] private float maxChargeTime = 2f; // Temps maximum de charge (optionnel)

    private float chargeTime = 0f;
    private bool isAiming = false;

    private void Update()
    {
        HandleAimingAndShooting();
    }

    private void HandleAimingAndShooting()
    {
        if (Input.GetMouseButtonDown(1)) // Clic droit maintenu pour viser
        {
            StartAiming();
        }

        if (Input.GetMouseButton(1)) // Continue de charger la flèche
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime); // Optionnel : Limite la charge maximale
        }

        if (Input.GetMouseButtonUp(1)) // Relâche le clic pour tirer
        {
            ShootArrow();
        }
    }

    private void StartAiming()
    {
        isAiming = true;
        chargeTime = 0f;

        // Désactiver les mouvements (Supposons que votre script de mouvement s'appelle "CharacterMovement")
        var movementScript = GetComponent<CharacterMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        Debug.Log("Aiming started");
    }

    private void ShootArrow()
    {
        if (!isAiming) return;

        isAiming = false;

        // Réactiver les mouvements
        var movementScript = GetComponent<CharacterMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }

        // Instancie une flèche
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        // Ajuste la rotation de la flèche pour correspondre à celle du joueur
        arrow.transform.forward = transform.forward;

        // Ajoute une force à la flèche pour la propulser
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // La vitesse est modulée en fonction du temps de charge
            float finalSpeed = arrowSpeed * (1 + chargeTime / maxChargeTime);
            rb.velocity = transform.forward * finalSpeed;
        }

        Debug.Log($"Arrow fired with charge time: {chargeTime}");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // Pr�fab de la fl�che
    [SerializeField] private Transform arrowSpawnPoint; // Point d'apparition des fl�ches
    [SerializeField] private float arrowSpeed = 20f; // Vitesse de la fl�che
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

        if (Input.GetMouseButton(1)) // Continue de charger la fl�che
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime); // Optionnel : Limite la charge maximale
        }

        if (Input.GetMouseButtonUp(1)) // Rel�che le clic pour tirer
        {
            ShootArrow();
        }
    }

    private void StartAiming()
    {
        isAiming = true;
        chargeTime = 0f;

        // D�sactiver les mouvements (Supposons que votre script de mouvement s'appelle "CharacterMovement")
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

        // R�activer les mouvements
        var movementScript = GetComponent<CharacterMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }

        // Instancie une fl�che
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        // Ajuste la rotation de la fl�che pour correspondre � celle du joueur
        arrow.transform.forward = transform.forward;

        // Ajoute une force � la fl�che pour la propulser
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // La vitesse est modul�e en fonction du temps de charge
            float finalSpeed = arrowSpeed * (1 + chargeTime / maxChargeTime);
            rb.velocity = transform.forward * finalSpeed;
        }

        Debug.Log($"Arrow fired with charge time: {chargeTime}");
    }
}
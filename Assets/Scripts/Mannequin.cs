using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour
{
    public GameObject hitVFX;          // Effet visuel d'impact
    public float impactForce = 5f;      // Force de l'impact
    public int hitsToDetach = 5;        // Nombre de coups nécessaires pour détacher une partie
    public int maxHealth = 100;         // Points de vie du mannequin
    public int damagePerHit = 10;       // Dégâts infligés par coup
    public GameObject destructionVFX;  // Effet visuel de destruction (ex: fumée, explosion)
    public GameObject mannequinPrefab; // Préfab pour respawn du mannequin
    public float respawnDelay = 3f;     // Temps avant respawn

    private Rigidbody rb;
    private FixedJoint[] joints;        // Tableau des FixedJoint attachés au mannequin
    private int hitCount = 0;           // Compteur de coups pour détachement
    private int currentHealth;          // Points de vie actuels
    public Vector3 initialPosition; // Ajouter cette variable pour stocker la position initiale du mannequin
    public Quaternion initialRotation;

    public List<GameObject> detachedParts = new List<GameObject>(); // Liste des parties détachées

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Récupérer tous les FixedJoint attachés aux objets enfants
        joints = GetComponentsInChildren<FixedJoint>();

        // Initialiser les points de vie
        currentHealth = maxHealth;

        // Enregistrer la position et la rotation initiale
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Ajouter les parties détachées à la liste
        detachedParts.Clear();
        foreach (FixedJoint joint in joints)
        {
            detachedParts.Add(joint.gameObject);
        }
    }

    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            TakeHit(hit);
        }
    }

    void TakeHit(RaycastHit hit)
    {
        // Jouer l'effet visuel à l'endroit de l'impact
        if (hitVFX != null)
        {
            GameObject impactEffect = Instantiate(hitVFX, hit.point, Quaternion.identity);

            // Détruire l'effet après un certain délai (par exemple 2 secondes)
            Destroy(impactEffect, 2f);
        }

        // Appliquer une force à l'endroit de l'impact
        Vector3 impactDirection = hit.point - transform.position;
        impactDirection = impactDirection.normalized;

        if (rb != null)
        {
            rb.AddForceAtPosition(impactDirection * impactForce, hit.point, ForceMode.Impulse);
        }

        // Réduire les points de vie
        currentHealth -= damagePerHit;
        Debug.Log("Mannequin HP: " + currentHealth);

        // Augmenter le compteur de coups
        hitCount++;

        // Tous les "hitsToDetach" coups, détacher une partie
        if (hitCount >= hitsToDetach)
        {
            DetachPart();
            hitCount = 0; // Réinitialiser le compteur de détachement
        }

        // Détruire le mannequin si sa vie tombe à zéro
        if (currentHealth <= 0)
        {
            StartCoroutine(DestroyAndRespawn());
        }
    }

    void DetachPart()
    {
        foreach (FixedJoint joint in joints)
        {
            if (joint == GetComponent<FixedJoint>()) // Ne pas détacher le body principal
                continue;

            if (joint != null)
            {
                Destroy(joint); // Supprime le FixedJoint pour détacher l'objet
                Debug.Log("Une partie du mannequin est tombée !");
                joints = GetComponentsInChildren<FixedJoint>(); // Mise à jour de la liste des joints
                return;
            }
        }

        Debug.Log("Toutes les parties du mannequin sont déjà détachées !");
    }

    IEnumerator DestroyAndRespawn()
    {
        Debug.Log("Destruction du mannequin...");

        // Jouer l'effet de destruction visuelle (ex: explosion, fumée)
        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }

        // Laisser un délai pour l'effet visuel de destruction
        yield return new WaitForSeconds(1f);

        // Détruire tout le mannequin (lui-même et ses enfants)
        DestroyMannequin();

        yield return new WaitForSeconds(respawnDelay); // Temps avant respawn

        Debug.Log("Tentative de respawn...");

        // Respawn du mannequin
        Respawn();
    }

    void DestroyMannequin()
    {
        // Détruire le mannequin et tous ses objets enfants (y compris les objets détachés)
        Debug.Log("Destruction du mannequin en cours...");
        Destroy(gameObject);
    }

    void Respawn()
    {
        Debug.Log("Respawn du mannequin...");

        // Instancier un nouveau mannequin à la position initiale
        if (mannequinPrefab != null)
        {
            GameObject newMannequin = Instantiate(mannequinPrefab, initialPosition, initialRotation);
            newMannequin.GetComponent<Mannequin>().ResetMannequin(); // Réinitialiser le mannequin
        }
        else
        {
            Debug.LogError("Prefab du mannequin non défini !");
        }
    }

    void ResetMannequin()
    {
        Debug.Log("Réinitialisation du mannequin...");

        currentHealth = maxHealth;
        hitCount = 0;
        joints = GetComponentsInChildren<FixedJoint>(); // Réactualiser les joints
        rb.velocity = Vector3.zero; // Annuler toute vitesse résiduelle
        rb.angularVelocity = Vector3.zero; // Annuler la vitesse angulaire
        Debug.Log("Mannequin réinitialisé !");
    }
}
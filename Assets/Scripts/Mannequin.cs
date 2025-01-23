using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour
{
    public GameObject hitVFX;          // Effet visuel d'impact
    public float impactForce = 5f;      // Force de l'impact
    public int hitsToDetach = 5;        // Nombre de coups n�cessaires pour d�tacher une partie
    public int maxHealth = 100;         // Points de vie du mannequin
    public int damagePerHit = 10;       // D�g�ts inflig�s par coup
    public GameObject destructionVFX;  // Effet visuel de destruction (ex: fum�e, explosion)
    public GameObject mannequinPrefab; // Pr�fab pour respawn du mannequin
    public float respawnDelay = 3f;     // Temps avant respawn

    private Rigidbody rb;
    private FixedJoint[] joints;        // Tableau des FixedJoint attach�s au mannequin
    private int hitCount = 0;           // Compteur de coups pour d�tachement
    private int currentHealth;          // Points de vie actuels
    public Vector3 initialPosition; // Ajouter cette variable pour stocker la position initiale du mannequin
    public Quaternion initialRotation;

    public List<GameObject> detachedParts = new List<GameObject>(); // Liste des parties d�tach�es

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // R�cup�rer tous les FixedJoint attach�s aux objets enfants
        joints = GetComponentsInChildren<FixedJoint>();

        // Initialiser les points de vie
        currentHealth = maxHealth;

        // Enregistrer la position et la rotation initiale
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Ajouter les parties d�tach�es � la liste
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
        // Jouer l'effet visuel � l'endroit de l'impact
        if (hitVFX != null)
        {
            GameObject impactEffect = Instantiate(hitVFX, hit.point, Quaternion.identity);

            // D�truire l'effet apr�s un certain d�lai (par exemple 2 secondes)
            Destroy(impactEffect, 2f);
        }

        // Appliquer une force � l'endroit de l'impact
        Vector3 impactDirection = hit.point - transform.position;
        impactDirection = impactDirection.normalized;

        if (rb != null)
        {
            rb.AddForceAtPosition(impactDirection * impactForce, hit.point, ForceMode.Impulse);
        }

        // R�duire les points de vie
        currentHealth -= damagePerHit;
        Debug.Log("Mannequin HP: " + currentHealth);

        // Augmenter le compteur de coups
        hitCount++;

        // Tous les "hitsToDetach" coups, d�tacher une partie
        if (hitCount >= hitsToDetach)
        {
            DetachPart();
            hitCount = 0; // R�initialiser le compteur de d�tachement
        }

        // D�truire le mannequin si sa vie tombe � z�ro
        if (currentHealth <= 0)
        {
            StartCoroutine(DestroyAndRespawn());
        }
    }

    void DetachPart()
    {
        foreach (FixedJoint joint in joints)
        {
            if (joint == GetComponent<FixedJoint>()) // Ne pas d�tacher le body principal
                continue;

            if (joint != null)
            {
                Destroy(joint); // Supprime le FixedJoint pour d�tacher l'objet
                Debug.Log("Une partie du mannequin est tomb�e !");
                joints = GetComponentsInChildren<FixedJoint>(); // Mise � jour de la liste des joints
                return;
            }
        }

        Debug.Log("Toutes les parties du mannequin sont d�j� d�tach�es !");
    }

    IEnumerator DestroyAndRespawn()
    {
        Debug.Log("Destruction du mannequin...");

        // Jouer l'effet de destruction visuelle (ex: explosion, fum�e)
        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }

        // Laisser un d�lai pour l'effet visuel de destruction
        yield return new WaitForSeconds(1f);

        // D�truire tout le mannequin (lui-m�me et ses enfants)
        DestroyMannequin();

        yield return new WaitForSeconds(respawnDelay); // Temps avant respawn

        Debug.Log("Tentative de respawn...");

        // Respawn du mannequin
        Respawn();
    }

    void DestroyMannequin()
    {
        // D�truire le mannequin et tous ses objets enfants (y compris les objets d�tach�s)
        Debug.Log("Destruction du mannequin en cours...");
        Destroy(gameObject);
    }

    void Respawn()
    {
        Debug.Log("Respawn du mannequin...");

        // Instancier un nouveau mannequin � la position initiale
        if (mannequinPrefab != null)
        {
            GameObject newMannequin = Instantiate(mannequinPrefab, initialPosition, initialRotation);
            newMannequin.GetComponent<Mannequin>().ResetMannequin(); // R�initialiser le mannequin
        }
        else
        {
            Debug.LogError("Prefab du mannequin non d�fini !");
        }
    }

    void ResetMannequin()
    {
        Debug.Log("R�initialisation du mannequin...");

        currentHealth = maxHealth;
        hitCount = 0;
        joints = GetComponentsInChildren<FixedJoint>(); // R�actualiser les joints
        rb.velocity = Vector3.zero; // Annuler toute vitesse r�siduelle
        rb.angularVelocity = Vector3.zero; // Annuler la vitesse angulaire
        Debug.Log("Mannequin r�initialis� !");
    }
}
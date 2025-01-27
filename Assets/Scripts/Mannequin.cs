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
    public AudioSource hitSound;        // Son d'impact lors d'un coup

    private Rigidbody rb;
    private FixedJoint[] joints;        // Tableau des FixedJoint attach�s au mannequin
    private int hitCount = 0;           // Compteur de coups pour d�tachement
    public int currentHealth;           // Points de vie actuels
    public Vector3 initialPosition;     // Position initiale du mannequin
    public Quaternion initialRotation;

    public List<GameObject> detachedParts = new List<GameObject>(); // Liste des parties d�tach�es
    public MannequinVie healthUI;
    public GameObject healthBar;

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
        currentHealth -= damagePerHit;

        if (healthUI != null)
        {
            healthUI.healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DestroyAndRespawn());
        }

        // Jouer l'effet visuel � l'endroit de l'impact
        if (hitVFX != null)
        {
            GameObject impactEffect = Instantiate(hitVFX, hit.point, Quaternion.identity);
            Destroy(impactEffect, 2f);  // D�truire l'effet apr�s un d�lai
        }

        // Jouer le son d'impact
        if (hitSound != null)
        {
            hitSound.Play();
        }

        // Appliquer une force � l'endroit de l'impact
        Vector3 impactDirection = hit.point - transform.position;
        impactDirection = impactDirection.normalized;

        if (rb != null)
        {
            rb.AddForceAtPosition(impactDirection * impactForce, hit.point, ForceMode.Impulse);
        }

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
            if (joint == GetComponent<FixedJoint>()) // Ne pas d�tacher le corps principal
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

        yield return new WaitForSeconds(1f);

        DestroyMannequin();

        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("Tentative de respawn...");

        Respawn();
    }

    void DestroyMannequin()
    {
        Debug.Log("Destruction du mannequin en cours...");

        if (healthBar != null)
        {
            Destroy(healthBar);  // D�truire la barre de vie si elle existe
        }

        Destroy(gameObject);
    }

    void Respawn()
    {
        Debug.Log("Respawn du mannequin...");

        if (mannequinPrefab != null)
        {
            GameObject newMannequin = Instantiate(mannequinPrefab, initialPosition, initialRotation);
            newMannequin.GetComponent<Mannequin>().ResetMannequin();
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
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Debug.Log("Mannequin r�initialis� !");
    }
}
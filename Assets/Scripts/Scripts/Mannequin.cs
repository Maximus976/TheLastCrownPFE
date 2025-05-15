using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour
{
    public GameObject hitVFX;          // Effet visuel d'impact
    public float impactForce = 0f;      // Force de l'impact (désactivée)
    public int hitsToDetach = 5;        // Nombre de coups nécessaires pour détacher une partie
    public int maxHealth = 100;         // Points de vie du mannequin
    public int damagePerHit = 10;       // Dégâts infligés par coup
    public GameObject destructionVFX;  // Effet visuel de destruction (ex: fumée, explosion)
    public AudioSource hitSound;        // Son d'impact lors d'un coup

    private Rigidbody rb;
    private FixedJoint[] joints;        // Tableau des FixedJoint attachés au mannequin
    private int hitCount = 0;           // Compteur de coups pour détachement
    public int currentHealth;           // Points de vie actuels
    public Vector3 initialPosition;     // Position initiale du mannequin
    public Quaternion initialRotation;

    public List<GameObject> detachedParts = new List<GameObject>(); // Liste des parties détachées
    public MannequinVie healthUI;
    public GameObject healthBar;

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

    public void TakeHit(Vector3 hitPoint)
    {
        currentHealth -= damagePerHit;

        if (healthUI != null)
        {
            healthUI.healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            DestroyMannequin();
        }

        // Jouer l'effet visuel à l'endroit de l'impact
        if (hitVFX != null)
        {
            GameObject impactEffect = Instantiate(hitVFX, hitPoint, Quaternion.identity);
            Destroy(impactEffect, 2f);  // Détruire l'effet après un délai
        }

        // Jouer le son d'impact
        if (hitSound != null)
        {
            hitSound.Play();
        }

        // Augmenter le compteur de coups
        hitCount++;

        // Tous les "hitsToDetach" coups, détacher une partie
        if (hitCount >= hitsToDetach)
        {
            DetachPart();
            hitCount = 0; // Réinitialiser le compteur de détachement
        }
    }

    void DetachPart()
    {
        foreach (FixedJoint joint in joints)
        {
            if (joint == GetComponent<FixedJoint>()) // Ne pas détacher le corps principal
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

    void DestroyMannequin()
    {
        Debug.Log("Destruction du mannequin...");

        // Jouer l'effet de destruction visuelle (ex: explosion, fumée)
        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }

        if (healthBar != null)
        {
            Destroy(healthBar);  // Détruire la barre de vie si elle existe
        }

        Destroy(gameObject);
    }
}
 
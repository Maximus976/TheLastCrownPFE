using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour
{
    public GameObject hitVFX;          // Effet visuel d'impact
    public float impactForce = 0f;      // Force de l'impact (d�sactiv�e)
    public int hitsToDetach = 5;        // Nombre de coups n�cessaires pour d�tacher une partie
    public int maxHealth = 100;         // Points de vie du mannequin
    public int damagePerHit = 10;       // D�g�ts inflig�s par coup
    public GameObject destructionVFX;  // Effet visuel de destruction (ex: fum�e, explosion)
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

        // Jouer l'effet visuel � l'endroit de l'impact
        if (hitVFX != null)
        {
            GameObject impactEffect = Instantiate(hitVFX, hitPoint, Quaternion.identity);
            Destroy(impactEffect, 2f);  // D�truire l'effet apr�s un d�lai
        }

        // Jouer le son d'impact
        if (hitSound != null)
        {
            hitSound.Play();
        }

        // Augmenter le compteur de coups
        hitCount++;

        // Tous les "hitsToDetach" coups, d�tacher une partie
        if (hitCount >= hitsToDetach)
        {
            DetachPart();
            hitCount = 0; // R�initialiser le compteur de d�tachement
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

    void DestroyMannequin()
    {
        Debug.Log("Destruction du mannequin...");

        // Jouer l'effet de destruction visuelle (ex: explosion, fum�e)
        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }

        if (healthBar != null)
        {
            Destroy(healthBar);  // D�truire la barre de vie si elle existe
        }

        Destroy(gameObject);
    }
}
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levier : MonoBehaviour
{
    public Grille grille;
    public Transform manche;                  // Le manche à déplacer
    public Transform pivotRotation;           // Un empty placé au centre du cercle du levier
    public GameObject uiInteraction;
    public float angleDepart = -20f;          // Angle initial
    public float angleFinal = 40f;            // Angle final
    public float vitesseRotation = 60f;       // Degrés par seconde
    public float cooldown = 1f;

    private bool joueurDansZone = false;
    private bool levierActive = false;
    private float angleActuel;
    private float angleCible;
    private float dernierAppui = -1f;
    private float rayon;

    void Start()
    {
        // Calculer le rayon initial
        rayon = Vector3.Distance(pivotRotation.position, manche.position);
        angleActuel = angleDepart;
        angleCible = angleDepart;
    }

    void Update()
    {
        if (joueurDansZone && Input.GetKeyDown(KeyCode.E) && Time.time - dernierAppui > cooldown)
        {
            dernierAppui = Time.time;
            levierActive = !levierActive;
            angleCible = levierActive ? angleFinal : angleDepart;

            if (grille != null)
                grille.ActiverGrille();
        }

        // Faire suivre un arc circulaire
        if (Mathf.Abs(angleActuel - angleCible) > 0.1f)
        {
            float direction = Mathf.Sign(angleCible - angleActuel);
            angleActuel += direction * vitesseRotation * Time.deltaTime;
            angleActuel = Mathf.Clamp(angleActuel, Mathf.Min(angleDepart, angleFinal), Mathf.Max(angleDepart, angleFinal));

            // Calculer la nouvelle position du manche sur l’arc circulaire
            float angleRad = angleActuel * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(angleRad), Mathf.Cos(angleRad), 0) * rayon;
            manche.position = pivotRotation.position + offset;

            // Tourner le manche pour qu’il reste orienté vers le centre
            manche.LookAt(pivotRotation.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = true;
            if (uiInteraction != null) uiInteraction.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            if (uiInteraction != null) uiInteraction.SetActive(false);
        }
    }
}
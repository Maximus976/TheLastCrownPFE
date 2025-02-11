using UnityEngine;

public class Grille : MonoBehaviour
{
    // Variable statique pour suivre la grille actuellement ouverte (s'il y en a une)
    public static Grille grilleOuverte = null;

    public Transform[] barreaux;          // Les 5 barreaux
    public Vector3[] positionsFermees;    // Positions locales fermées
    public Vector3[] positionsOuvertes;     // Positions locales ouvertes
    public float vitesse = 2f;            // Vitesse de mouvement

    private Vector3[] destinations;
    private bool enMouvement = false;
    private bool estOuverte = false;      // Etat de la grille
    private Collider[] colliders;         // Les colliders des barreaux

    void Start()
    {
        // Initialisation
        destinations = new Vector3[barreaux.Length];
        colliders = new Collider[barreaux.Length];

        for (int i = 0; i < barreaux.Length; i++)
        {
            // Forcer la position fermée au départ
            barreaux[i].localPosition = positionsFermees[i];
            destinations[i] = positionsFermees[i];

            // Récupérer les colliders de chaque barreau
            colliders[i] = barreaux[i].GetComponent<Collider>();
            if (colliders[i] != null)
                colliders[i].enabled = true;
        }

        estOuverte = false; // La grille commence fermée
    }

    void Update()
    {
        // Par exemple, on ouvre avec "O" et ferme avec "F"
        if (Input.GetKeyDown(KeyCode.O) && !enMouvement && !estOuverte)
        {
            OuvrirGrille();
        }
        else if (Input.GetKeyDown(KeyCode.F) && !enMouvement && estOuverte)
        {
            FermerGrille();
        }

        // Animation du mouvement
        if (enMouvement)
        {
            bool tousArrives = true;
            for (int i = 0; i < barreaux.Length; i++)
            {
                barreaux[i].localPosition = Vector3.MoveTowards(barreaux[i].localPosition, destinations[i], vitesse * Time.deltaTime);
                if (Vector3.Distance(barreaux[i].localPosition, destinations[i]) > 0.01f)
                {
                    tousArrives = false;
                }
            }

            if (tousArrives)
            {
                enMouvement = false;

                // Si la grille est ouverte, on désactive les colliders
                if (estOuverte)
                {
                    foreach (Collider col in colliders)
                        col.enabled = false;
                }
                // Si la grille est fermée, on laisse les colliders activés
            }
        }
    }

    public void OuvrirGrille()
    {
        // Si une autre grille est déjà ouverte, la fermer
        if (grilleOuverte != null && grilleOuverte != this)
        {
            grilleOuverte.FermerGrille();
        }

        for (int i = 0; i < barreaux.Length; i++)
        {
            destinations[i] = positionsOuvertes[i];
        }
        enMouvement = true;
        estOuverte = true;

        // Une fois ouverte, on note cette grille comme étant celle ouverte
        grilleOuverte = this;
    }

    public void FermerGrille()
    {
        for (int i = 0; i < barreaux.Length; i++)
        {
            destinations[i] = positionsFermees[i];
        }
        enMouvement = true;
        estOuverte = false;

        // Dès que la fermeture commence, réactiver immédiatement les colliders
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        // Si cette grille était marquée comme ouverte, on la "détache"
        if (grilleOuverte == this)
        {
            grilleOuverte = null;
        }
    }
}

using UnityEngine;

public class EnemyDamageZone : MonoBehaviour
{
    [Header("Dégâts")]
    [SerializeField] private int damage = 10;

    private bool canDealDamage = false;
    private EnemyMovement enemy;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyMovement>();
        if (enemy == null)
        {
            Debug.LogWarning("EnemyMovement non trouvé dans le parent !");
        }

        gameObject.SetActive(false); // désactivée au départ
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponentInChildren<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("Dégâts infligés au joueur pendant l'animation !");
                canDealDamage = false; // une seule fois par attaque
            }
        }
    }

    public void ActivateHitbox()
    {
        canDealDamage = true;
        gameObject.SetActive(true);
    }

    public void DeactivateHitbox()
    {
        canDealDamage = false;
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyStone : MonoBehaviour
{
    public GameObject dustEffectPrefab;
    public float delayBeforeStart = 1f;
    public float timeBetweenGroups = 0.4f;
    public int stonesPerGroup = 3;
    public float fallForce = 10f;  // Force de chute pour un effet lourd
    public float maxRotationSpeed = 50f;  // Vitesse de rotation
    public float maxFallHeight = 1f;  // Hauteur maximale de chute (évite qu'elles ne s'envolent trop)

    void Start()
    {
        StartCoroutine(DestroyStonesInGroups());
    }

    private IEnumerator DestroyStonesInGroups()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        // Stocke toutes les pierettes dans une liste
        List<Transform> stones = new List<Transform>();
        foreach (Transform child in transform)
        {
            stones.Add(child);
        }

        int i = 0;
        while (i < stones.Count)
        {
            // Prend un groupe de pierres
            for (int j = 0; j < stonesPerGroup && i < stones.Count; j++, i++)
            {
                Transform stone = stones[i];

                if (dustEffectPrefab != null)
                {
                    Vector3 spawnPos = stone.position + Vector3.up * Random.Range(-0.2f, 0.2f);
                    GameObject dust = Instantiate(dustEffectPrefab, spawnPos, Quaternion.identity);
                    Destroy(dust, 5f);  // Détruire après un certain temps
                }

                // Applique une force de chute "lourde" pour l'effet
                Rigidbody rb = stone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;  // Active la physique
                    rb.AddForce(Vector3.down * fallForce, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * maxRotationSpeed, ForceMode.Impulse);  // Rotation pour un effet d'écroulement
                }

                // Diminuer la taille de la pierre pour un effet de débris
                StartCoroutine(ScalePop(stone));

                // Retirer la pierre après un petit délai
                StartCoroutine(DelayedDeactivate(stone.gameObject, 0.2f));
            }

            yield return new WaitForSeconds(timeBetweenGroups);
        }

        // Détruire le mur une fois toutes les pierres effondrées
        Destroy(gameObject);
    }

    private IEnumerator DelayedDeactivate(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);  // Désactive l'objet pour simuler l'éboulement
    }

    private IEnumerator ScalePop(Transform target)
    {
        Vector3 originalScale = target.localScale;
        Vector3 popScale = originalScale * 1.2f;  // Grossir un peu avant la disparition

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            target.localScale = Vector3.Lerp(originalScale, popScale, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }

        target.localScale = originalScale;  // Restaure la taille normale
    }
}
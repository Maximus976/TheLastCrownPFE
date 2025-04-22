using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurEboulement : MonoBehaviour
{
    public GameObject dustEffectPrefab;
    public float delayBeforeStart = 1f;
    public float timeBetweenGroups = 0.4f;
    public int stonesPerGroup = 3;

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
                    Destroy(dust, 5f);
                }

                StartCoroutine(ScalePop(stone));
                StartCoroutine(DelayedDeactivate(stone.gameObject, 0.2f));
            }

            yield return new WaitForSeconds(timeBetweenGroups);
        }

        Destroy(gameObject); // On détruit le mur après tout
    }

    private IEnumerator DelayedDeactivate(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private IEnumerator ScalePop(Transform target)
    {
        Vector3 originalScale = target.localScale;
        Vector3 popScale = originalScale * 1.2f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            target.localScale = Vector3.Lerp(originalScale, popScale, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }

        target.localScale = originalScale;
    }
}
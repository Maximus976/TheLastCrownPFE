using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpawn : MonoBehaviour
{
    [Header("Références")]
    public GameObject[] windPrefabs; // Différents types de vent
    public Transform player;

    [Header("Paramètres de spawn")]
    public float spawnInterval = 1.5f;
    public float minSpawnRadius = 3f;
    public float maxSpawnRadius = 6f;
    public int maxWindCount = 10; // Nombre maximum de vents actifs en même temps

    [Header("Isométrie")]
    [Range(0f, 1f)]
    public float isoSkew = 0.2f;

    private List<GameObject> activeWinds = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(SpawnWind), 0f, spawnInterval);
    }

    private void SpawnWind()
    {
        if (windPrefabs.Length == 0 || player == null)
            return;

        // Angle uniquement entre 45° et 135° pour viser le haut du joueur
        float angle = Random.Range(45f, 135f) * Mathf.Deg2Rad;
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
        offset.y -= offset.x * isoSkew;

        Vector3 spawnPosition = player.position + offset;

        int index = Random.Range(0, windPrefabs.Length);
        GameObject chosenWind = windPrefabs[index];

        GameObject newWind = Instantiate(chosenWind, spawnPosition, Quaternion.Euler(0f, 90f, 0f));
        activeWinds.Add(newWind);

        if (activeWinds.Count > maxWindCount)
        {
            Destroy(activeWinds[0]);
            activeWinds.RemoveAt(0);
        }
    }
}
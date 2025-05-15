using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpawn : MonoBehaviour
{
    [Header("R�f�rences")]
    public GameObject[] windPrefabs; // Diff�rents types de vent
    public Transform player;

    [Header("Param�tres de spawn")]
    public float spawnInterval = 1.5f;
    public float minSpawnRadius = 3f;
    public float maxSpawnRadius = 6f;
    public int maxWindCount = 10; // Nombre maximum de vents actifs en m�me temps

    [Header("Isom�trie")]
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

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;

        // Corriger pour l'isoSkew dans Z si besoin (simuler un effet visuel)
        // offset.z -= offset.x * isoSkew;

        Vector3 spawnPosition = player.position + offset;

        // Y fixe pour rester � la bonne hauteur
        spawnPosition.y = player.position.y;
        int index = Random.Range(0, windPrefabs.Length);
        GameObject chosenWind = windPrefabs[index];

        GameObject newWind = Instantiate(chosenWind, spawnPosition, Quaternion.Euler(0f, 180f, 0f));
        activeWinds.Add(newWind);

        if (activeWinds.Count > maxWindCount)
        {
            Destroy(activeWinds[0]);
            activeWinds.RemoveAt(0);
        }
    }
}
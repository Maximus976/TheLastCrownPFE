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

    [Header("Isom�trie")]
    [Range(0f, 1f)]
    public float isoSkew = 0.2f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnWind), 0f, spawnInterval);
    }

    private void SpawnWind()
    {
        // Angle et distance al�atoires autour du joueur
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
        offset.y -= offset.x * isoSkew;

        Vector3 spawnPosition = player.position + offset;

        // Choisit un prefab al�atoire
        int index = Random.Range(0, windPrefabs.Length);
        GameObject chosenWind = windPrefabs[index];

        // Instancie avec une rotation de 90� sur Y
        Instantiate(chosenWind, spawnPosition, Quaternion.Euler(0f, 90f, 0f));
    }
}
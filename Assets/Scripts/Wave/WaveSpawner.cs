using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject enemyPrefab;
        public int count;
        public Transform[] spawnPoints;
        public float spawnDelay = 0.5f;
    }

    [System.Serializable]
    public class Wave
    {
        public string name;
        public List<EnemySpawn> enemiesToSpawn;
    }

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    private bool waveInProgress = false;

    void Update()
    {
        if (!waveInProgress && currentWaveIndex < waves.Count)
        {
            // Appuyer sur E pour lancer la vague suivante
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            }
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        waveInProgress = true;
        Debug.Log($"Lancement de la vague : {wave.name}");

        foreach (EnemySpawn enemySpawn in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                Transform spawnPoint = enemySpawn.spawnPoints[Random.Range(0, enemySpawn.spawnPoints.Length)];

                GameObject enemy = Instantiate(enemySpawn.enemyPrefab, spawnPoint.position, spawnPoint.rotation);

                // Si le script de déplacement est désactivé par défaut, tu peux l’activer ici :
                // enemy.GetComponent<NomDuScriptDeDeplacement>()?.enabled = true;

                yield return new WaitForSeconds(enemySpawn.spawnDelay);
            }
        }

        // Attente que tous les ennemis soient morts (tag corrigé : "ennemi")
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("ennemi").Length == 0);

        currentWaveIndex++;
        waveInProgress = false;
        Debug.Log("Vague terminée. Appuyez sur E pour la suivante.");
    }
}

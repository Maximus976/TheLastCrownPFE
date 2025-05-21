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

    private List<GameObject> currentEnemies = new List<GameObject>();

    [Header("Objets à faire descendre après la dernière vague")]
    public List<Transform> objetsADescendre;
    public float descenteDistance = 2f;
    public float descenteDuree = 1f;

    public void StartFirstWave()
    {
        if (!waveInProgress && currentWaveIndex == 0)
        {
            Debug.Log("Déclenchement de la première vague");
            StartCoroutine(SpawnWave(currentWaveIndex));
        }
    }

    IEnumerator SpawnWave(int waveIndex)
    {
        waveInProgress = true;
        currentEnemies.Clear();

        Wave wave = waves[waveIndex];
        Debug.Log($"Lancement de la vague {waveIndex + 1} : {wave.name}");

        foreach (EnemySpawn enemySpawn in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                Transform spawnPoint = enemySpawn.spawnPoints[Random.Range(0, enemySpawn.spawnPoints.Length)];
                GameObject enemy = Instantiate(enemySpawn.enemyPrefab, spawnPoint.position, spawnPoint.rotation);

                currentEnemies.Add(enemy);

                EnemyMovement movement = enemy.GetComponentInChildren<EnemyMovement>();
                if (movement != null) movement.enabled = true;

                EnemyDeathHandler deathHandler = enemy.GetComponent<EnemyDeathHandler>();
                if (deathHandler != null) deathHandler.enabled = true;

                yield return new WaitForSeconds(enemySpawn.spawnDelay);
            }
        }

        Debug.Log("Attente que tous les ennemis de la vague soient éliminés...");
        yield return new WaitUntil(() => TousLesEnnemisElimines());

        Debug.Log($"Vague {waveIndex + 1} terminée.");

        currentWaveIndex++;
        waveInProgress = false;

        if (currentWaveIndex < waves.Count)
        {
            yield return new WaitForSeconds(2f);
            StartCoroutine(SpawnWave(currentWaveIndex));
        }
        else
        {
            Debug.Log("Toutes les vagues sont terminées ! Descente des objets...");

            foreach (Transform obj in objetsADescendre)
            {
                if (obj != null)
                    StartCoroutine(FaireDescendreObjet(obj));
            }
        }
    }

    private bool TousLesEnnemisElimines()
    {
        currentEnemies.RemoveAll(e => e == null);
        return currentEnemies.Count == 0;
    }

    IEnumerator FaireDescendreObjet(Transform obj)
    {
        Vector3 start = obj.position;
        Vector3 end = start - new Vector3(0f, descenteDistance, 0f);
        float elapsed = 0f;

        while (elapsed < descenteDuree)
        {
            obj.position = Vector3.Lerp(start, end, elapsed / descenteDuree);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = end; // forcer la position finale
    }
}

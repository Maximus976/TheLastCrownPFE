using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

    [System.Serializable]
    public class DescenteGroupe
    {
        public Transform[] objets; // Objets à descendre ensemble
        public CinemachineVirtualCamera camera; // Caméra du groupe
    }

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    private bool waveInProgress = false;

    private List<GameObject> currentEnemies = new List<GameObject>();

    [Header("Groupes de descente")]
    public List<DescenteGroupe> groupesDeDescente;
    public float descenteDistance = 2f;
    public float descenteDuree = 2f;

    [Header("Caméra par défaut")]
    public CinemachineVirtualCamera defaultCamera;

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
            if (enemySpawn.spawnPoints.Length < enemySpawn.count)
            {
                Debug.LogError($"Pas assez de spawn points pour {enemySpawn.enemyPrefab.name}. Points : {enemySpawn.spawnPoints.Length}, ennemis : {enemySpawn.count}");
                yield break;
            }

            List<Transform> shuffledPoints = new List<Transform>(enemySpawn.spawnPoints);
            Shuffle(shuffledPoints);

            for (int i = 0; i < enemySpawn.count; i++)
            {
                Transform spawnPoint = shuffledPoints[i];
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
            Debug.Log("Toutes les vagues sont terminées !");
            yield return StartCoroutine(DescenteParGroupe());
            Debug.Log("Tous les objets ont été descendus.");
        }
    }

    private IEnumerator DescenteParGroupe()
    {
        foreach (var groupe in groupesDeDescente)
        {
            // 1. Active la caméra du groupe
            if (groupe.camera != null)
                groupe.camera.Priority = 20;

            if (defaultCamera != null)
                defaultCamera.Priority = 10;

            // 2. Attendre que la caméra prenne effet
            yield return new WaitForSeconds(0.3f);

            // 3. Lancer la descente de tous les objets
            List<Coroutine> descentes = new List<Coroutine>();
            foreach (Transform obj in groupe.objets)
            {
                if (obj != null)
                    descentes.Add(StartCoroutine(FaireDescenteEtAttendre(obj)));
            }

            // 4. Attendre la durée de la descente
            yield return new WaitForSeconds(descenteDuree);

            // 5. Revenir à la caméra de gameplay
            if (defaultCamera != null)
                defaultCamera.Priority = 20;

            if (groupe.camera != null)
                groupe.camera.Priority = 10;

            yield return new WaitForSeconds(0.3f);
        }
    }

    private bool TousLesEnnemisElimines()
    {
        currentEnemies.RemoveAll(e => e == null);
        return currentEnemies.Count == 0;
    }

    IEnumerator FaireDescenteEtAttendre(Transform obj)
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

        obj.position = end;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}

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
        public Transform[] objets;
        public CinemachineVirtualCamera camera;
        public float descenteDistance = 10f;
        public float descenteDuree = 2f;
    }

    [System.Serializable]
    public class MonteeGroupe
    {
        public Transform[] objets;
        public CinemachineVirtualCamera camera;
        public float monteeDistance = 10f;
        public float monteeDuree = 2f;
    }

    public List<Wave> waves;
    private int currentWaveIndex = 0;
    private bool waveInProgress = false;

    private List<GameObject> currentEnemies = new List<GameObject>();

    [Header("Groupes de montée")]
    public List<MonteeGroupe> groupesDeMontee;

    [Header("Groupes de descente finale")]
    public List<DescenteGroupe> groupesDeDescente;

    [Header("Caméra par défaut")]
    public CinemachineVirtualCamera defaultCamera;

    // Stocke les positions de départ
    private Dictionary<Transform, Vector3> positionsInitiales = new Dictionary<Transform, Vector3>();

    private void Start()
    {
        StartCoroutine(DescenteInitiale());
    }

    private IEnumerator DescenteInitiale()
    {
        foreach (var groupe in groupesDeMontee)
        {
            foreach (Transform obj in groupe.objets)
            {
                if (obj != null)
                {
                    // Sauvegarder la position de départ
                    if (!positionsInitiales.ContainsKey(obj))
                        positionsInitiales[obj] = obj.position;

                    // Déplacer vers le bas
                    obj.position -= new Vector3(0f, groupe.monteeDistance, 0f);
                }
            }
        }
        yield return null;
    }

    public void StartFirstWave()
    {
        if (!waveInProgress && currentWaveIndex == 0)
        {
            Debug.Log("Début des vagues : montée des objets...");
            StartCoroutine(LancerVagueAvecMontée());
        }
    }

    private IEnumerator LancerVagueAvecMontée()
    {
        yield return StartCoroutine(MonteeParGroupe());

        Debug.Log("Montée terminée. Début de la première vague !");
        StartCoroutine(SpawnWave(currentWaveIndex));
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

    private IEnumerator MonteeParGroupe()
    {
        foreach (var groupe in groupesDeMontee)
        {
            if (groupe.camera != null) groupe.camera.Priority = 20;
            if (defaultCamera != null) defaultCamera.Priority = 10;

            yield return new WaitForSeconds(0.3f);

            List<Coroutine> animations = new List<Coroutine>();
            foreach (Transform obj in groupe.objets)
            {
                if (obj != null && positionsInitiales.ContainsKey(obj))
                    animations.Add(StartCoroutine(DeplacementVersPosition(obj, positionsInitiales[obj], groupe.monteeDuree)));
            }

            foreach (Coroutine anim in animations)
                yield return anim;

            if (defaultCamera != null) defaultCamera.Priority = 20;
            if (groupe.camera != null) groupe.camera.Priority = 10;

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator DescenteParGroupe()
    {
        foreach (var groupe in groupesDeDescente)
        {
            if (groupe.camera != null) groupe.camera.Priority = 20;
            if (defaultCamera != null) defaultCamera.Priority = 10;

            yield return new WaitForSeconds(0.3f);

            List<Coroutine> animations = new List<Coroutine>();
            foreach (Transform obj in groupe.objets)
            {
                if (obj != null)
                    animations.Add(StartCoroutine(FaireDescenteEtAttendre(obj, groupe.descenteDistance, groupe.descenteDuree)));
            }

            foreach (Coroutine anim in animations)
                yield return anim;

            if (defaultCamera != null) defaultCamera.Priority = 20;
            if (groupe.camera != null) groupe.camera.Priority = 10;

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator DeplacementVersPosition(Transform obj, Vector3 cible, float duree)
    {
        Vector3 start = obj.position;
        float elapsed = 0f;

        while (elapsed < duree)
        {
            obj.position = Vector3.Lerp(start, cible, elapsed / duree);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = cible;
    }

    IEnumerator FaireDescenteEtAttendre(Transform obj, float distance, float duree)
    {
        Vector3 start = obj.position;
        Vector3 end = start - new Vector3(0f, distance, 0f);
        float elapsed = 0f;

        while (elapsed < duree)
        {
            obj.position = Vector3.Lerp(start, end, elapsed / duree);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = end;
    }

    private bool TousLesEnnemisElimines()
    {
        currentEnemies.RemoveAll(e => e == null);
        return currentEnemies.Count == 0;
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

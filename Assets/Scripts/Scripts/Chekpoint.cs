using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chekpoint : MonoBehaviour
{
    public static Chekpoint Instance { get; private set; }

    private Vector3 checkpointPosition;
    private Quaternion checkpointRotation;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre les scènes si nécessaire
    }

    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        checkpointPosition = position;
        checkpointRotation = rotation;
        hasCheckpoint = true;
        Debug.Log("[CheckpointManager] Checkpoint défini à : " + checkpointPosition);
    }

    public void RespawnPlayer(GameObject player)
    {
        if (!hasCheckpoint)
        {
            Debug.LogWarning("[CheckpointManager] Aucun checkpoint défini !");
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        // Téléporte le joueur au checkpoint
        player.transform.position = checkpointPosition;
        player.transform.rotation = checkpointRotation;

        if (cc != null)
            cc.enabled = true;

        // Réinitialise l’état de mort et la santé
        Health health = player.GetComponent<Health>();
        if (health != null)
            health.ResetDeathState();

        Debug.Log("[CheckpointManager] Joueur respawn proprement au checkpoint.");
    }
}

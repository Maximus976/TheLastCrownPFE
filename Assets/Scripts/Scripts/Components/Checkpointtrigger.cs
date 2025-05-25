using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpointtrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Chekpoint.Instance.SetCheckpoint(
                transform.position,
                other.transform.rotation // ou transform.rotation si tu veux imposer une rotation
            );

            Debug.Log("Checkpoint atteint !");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpawnManager : MonoBehaviour
{
    void Start()
    {
        string pointName = PlayerPrefs.GetString("BouclierSpawn", "");
        Debug.Log("Point de spawn : " + pointName);  // Vérifie si le nom du point est bien récupéré

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!string.IsNullOrEmpty(pointName))
        {
            GameObject point = GameObject.Find(pointName);
            if (point != null && player != null)
            {
                player.transform.position = point.transform.position;
                player.transform.rotation = point.transform.rotation;
            }
        }

        // Nettoyage des données
        PlayerPrefs.DeleteKey("NextSpawnPoint");
        PlayerPrefs.DeleteKey("SceneToLoad");
    }
}
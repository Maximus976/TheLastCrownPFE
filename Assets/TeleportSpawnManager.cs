using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpawnManager : MonoBehaviour
{
    void Start()
    {
        string pointName = PlayerPrefs.GetString("BouclierSpawn", "");
        Debug.Log("Point de spawn : " + pointName);  // V�rifie si le nom du point est bien r�cup�r�

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

        // Nettoyage des donn�es
        PlayerPrefs.DeleteKey("NextSpawnPoint");
        PlayerPrefs.DeleteKey("SceneToLoad");
    }
}
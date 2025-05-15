using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    void Start()
    {
        string pointName = PlayerPrefs.GetString("BouclierSpawn", "");
        if (!string.IsNullOrEmpty(pointName))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            TeleportPoint[] points = FindObjectsOfType<TeleportPoint>();

            foreach (var point in points)
            {
                if (point.spawnPointName == pointName)
                {
                    player.transform.position = point.transform.position;
                    player.transform.rotation = point.transform.rotation;
                    break;
                }
            }

            PlayerPrefs.DeleteKey("NextSpawnPoint");
            PlayerPrefs.DeleteKey("SceneToLoad");
        }
    }
}
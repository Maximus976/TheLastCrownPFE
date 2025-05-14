using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePlayerFoloow : MonoBehaviour
{
    public float speed = 5f;
    public float duration = 2f;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        Destroy(gameObject, duration); // l'effet disparaît après un temps
    }

    void Update()
    {
        if (player == null) return;

        // Déplacement fluide vers le joueur
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }
}
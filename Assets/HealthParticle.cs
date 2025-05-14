using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthParticle : MonoBehaviour
{
    public float speed = 5f;
    public float delayBeforeAttraction = 0.2f;
    public int healAmount = 10;

    private Transform player;
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private float timer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        ps.Play();
    }

    void LateUpdate()
    {
        if (player == null || ps == null) return;

        timer += Time.deltaTime;
        if (timer < delayBeforeAttraction) return;

        int aliveParticles = ps.GetParticles(particles);

        for (int i = 0; i < aliveParticles; i++)
        {
            Vector3 dir = (player.position - particles[i].position).normalized;
            particles[i].position += dir * speed * Time.deltaTime;
        }

        ps.SetParticles(particles, aliveParticles);
    }

    // ? Nécessite que "Send Collision Messages" soit activé dans le Particle System
    private void OnParticleCollision(GameObject other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.RestoreHealth(healAmount);
        }
    }
}
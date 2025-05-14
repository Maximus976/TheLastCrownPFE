using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poussieresouspied : MonoBehaviour
{
    public ParticleSystem dustEffect;
    public Rigidbody rb;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    public float walkSpeedThreshold = 1f;
    public float runSpeedThreshold = 4f;

    public float walkEmissionRate = 5f;
    public float runEmissionRate = 15f;

    private ParticleSystem.EmissionModule emission;

    void Start()
    {
        emission = dustEffect.emission;
        emission.rateOverTime = 0f;
        dustEffect.Stop();
    }

    void Update()
    {
        float speed = rb.velocity.magnitude;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && speed > 0.1f)
        {
            if (!dustEffect.isPlaying)
                dustEffect.Play();

            // Ajuste le taux selon la vitesse
            if (speed > runSpeedThreshold)
                emission.rateOverTime = runEmissionRate;
            else if (speed > walkSpeedThreshold)
                emission.rateOverTime = walkEmissionRate;
            else
                emission.rateOverTime = 0f;
        }
        else
        {
            if (dustEffect.isPlaying)
                dustEffect.Stop();
        }
    }
}
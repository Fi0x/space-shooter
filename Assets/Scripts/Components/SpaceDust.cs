using System;
using UnityEngine;
using Random = System.Random;

public class SpaceDust : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ShipMovementHandler smh;

    private const int ParticlesOverTime = 1000;
    private float minParticleSize = 0.1f;
    private float maxParticleSize = 1;

    void FixedUpdate()
    {
        var shipSpeed = smh.shipRigidbody.velocity.magnitude;
        
        var emission = particleSystem.emission;
        emission.rateOverTime = ParticlesOverTime / (smh.maxSpeed + smh.maxSpeedBoost) * shipSpeed;

        var main = particleSystem.main;
        main.startSize = (maxParticleSize - minParticleSize) / (smh.maxSpeed + smh.maxSpeedBoost) * shipSpeed + minParticleSize;
    }
}
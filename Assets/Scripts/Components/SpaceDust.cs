using System;
using Ship;
using UnityEngine;
using Random = System.Random;

public class SpaceDust : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ShipMovementHandler smh;

    private const int ParticlesOverTime = 100;
    private const float MinParticleSize = 0.1f;
    private const float MaxParticleSize = 0.2f;
    private const float MaxLifetime = 1;

    void FixedUpdate()
    {
        var maxShipSpeed = smh.maxSpeed + smh.maxSpeedBoost;
        var shipSpeed = smh.shipRigidbody.velocity.magnitude;
        
        var emission = particleSystem.emission;
        emission.rateOverTime = ParticlesOverTime / maxShipSpeed * shipSpeed;

        var main = particleSystem.main;
        main.startSize = (MaxParticleSize - MinParticleSize) / maxShipSpeed * shipSpeed + MinParticleSize;
        main.startLifetime = MaxLifetime * 1.1f - shipSpeed / maxShipSpeed;
    }
}
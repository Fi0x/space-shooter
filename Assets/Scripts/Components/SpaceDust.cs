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
        var maxShipSpeed = this.smh.Settings.MaxSpeed + this.smh.Settings.MaxSpeedBoost;
        var shipSpeed = this.smh.ShipRB.velocity.magnitude;
        
        var emission = this.particleSystem.emission;
        emission.rateOverTime = ParticlesOverTime / maxShipSpeed * shipSpeed;

        var main = this.particleSystem.main;
        main.startSize = (MaxParticleSize - MinParticleSize) / maxShipSpeed * shipSpeed + MinParticleSize;
        main.startLifetime = MaxLifetime * 1.1f - shipSpeed / maxShipSpeed;
    }
}
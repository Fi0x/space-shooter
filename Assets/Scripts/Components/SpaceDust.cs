using Ship;
using Ship.Movement;
using UnityEngine;

namespace Components
{
    public class SpaceDust : MonoBehaviour
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private PlayerShipMovementHandler smh;

        private const int ParticlesOverTime = 100;
        private const float MinParticleSize = 0.1f;
        private const float MaxParticleSize = 0.2f;
        private const float MaxLifetime = 1;

        void FixedUpdate()
        {
            var maxShipSpeed = this.smh.Settings.MaxSpeed + this.smh.Settings.MaxSpeedBoost;
            var shipSpeed = this.smh.ShipRb.velocity.magnitude;
        
            var emission = this.particleSystem.emission;
            emission.rateOverTime = ParticlesOverTime / maxShipSpeed * shipSpeed;

            var main = this.particleSystem.main;
            main.startSize = (MaxParticleSize - MinParticleSize) / maxShipSpeed * shipSpeed + MinParticleSize;
            main.startLifetime = MaxLifetime * 1.1f - shipSpeed / maxShipSpeed;
        }
    }
}
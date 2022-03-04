#nullable enable
using System;
using Manager;
using Ship.Weaponry.Config;
using UnityEngine;

namespace Targeting
{
    public class Targetable : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody = null!;
        
        private void OnEnable()
        {
            GameManager.Instance.TargetableManager.NotifyAboutNewTargetable(this);
            this.rigidbody ??= this.GetComponent<Rigidbody>() ?? throw new Exception("No Rigidbody on target");
        }

        private void OnDisable()
        {
            GameManager.Instance.TargetableManager.NotifyAboutTargetableGone(this);
        }



        public (Vector3 position, float travelTime, bool canHit)? GetPredictedTargetLocation(Vector3 shooterPosition,
            WeaponConfigScriptableObject weaponConfig)
        {
            if (weaponConfig is WeaponHitScanConfigScriptableObject hitscanSo)
            {
                float distanceToShooter = Vector3.Distance(shooterPosition, this.transform.position);
                
                return (this.gameObject.transform.position, 0f, hitscanSo.MaxDistance > distanceToShooter);
            }
            else if (weaponConfig is WeaponProjectileConfigScriptableObject projectileSo)
            {
                return this.GetPredictedTargetLocationProjectile(shooterPosition,
                    projectileSo.ProjectileSpeed, projectileSo.TimeToLive);
            }
            else
            {
                throw new Exception("Unsupported Weapon Config Type");
            }
        }

        private (Vector3 position, float travelTime, bool canHit)?  GetPredictedTargetLocationProjectile(Vector3 shooterPosition, float projectileSpeed, float ttl)
        {

            var timeOfCollision = this.GetPredictedTimeOfCollision(shooterPosition, projectileSpeed);
            if (timeOfCollision == null)
            {
                return null;
            }

            var position = this.transform.position + timeOfCollision.Value * this.rigidbody.velocity;
            return (position, timeOfCollision.Value, timeOfCollision.Value < ttl);
        }

        private float? GetPredictedTimeOfCollision(Vector3 shooterPosition, float projectileSpeed)
        {
            var deltaPosition = this.transform.position - shooterPosition;
            var ownMovement = this.rigidbody.velocity;
            if (ownMovement.x == 0)
            {
                return null; // prevent NaN
            } 
            var ownStartX = deltaPosition.x;
            var ownStartZ = deltaPosition.z;
            var ownMovementGradient = ownMovement.z / ownMovement.x;

            var sqrtContent = ownStartX * ownStartX * ownMovementGradient * ownMovementGradient * projectileSpeed * projectileSpeed 
                - ownStartX * ownStartX * ownMovementGradient * ownMovementGradient 
                - 2 * ownStartX * ownMovementGradient * ownStartZ * projectileSpeed * projectileSpeed 
                + 2 * ownStartX * ownMovementGradient * ownStartZ 
                + ownStartZ * ownStartZ * projectileSpeed * projectileSpeed 
                - ownStartZ * ownStartZ;
            var numerator = -Math.Sqrt(sqrtContent) + ownStartX * ownMovementGradient * ownMovementGradient 
                            - ownMovementGradient * ownStartZ;
            var denominator = ownMovementGradient * ownMovementGradient - projectileSpeed * projectileSpeed + 1;
            var result = numerator / denominator;

            if (result <= 0)
            {
                return null;
            }

            return (float)result;
        }
    }
}
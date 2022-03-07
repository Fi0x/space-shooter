#nullable enable
using System;
using Manager;
using Ship.Weaponry.Config;
using UnityEngine;

namespace Targeting
{
    public class Targetable : MonoBehaviour
    {

        private void OnEnable()
        {
            GameManager.Instance.TargetableManager.NotifyAboutNewTargetable(this);
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

            var ownMovement = this.GetOwnMovement();

            var position = this.transform.position + timeOfCollision.Value * ownMovement;
            return (position, timeOfCollision.Value, timeOfCollision.Value < ttl);
        }

        private Vector3 GetOwnMovement()
        {
            return Vector3.one * 2;// !!! Always 0 ?! (this.transform.position - positionLastFrame) / Time.deltaTime;
        }

        
        // Some black magic is happening here. Its pretty hard to get it from the code.
        // Please refer to this Desmos Page: https://www.desmos.com/calculator/jthl2vjkps
        private float? GetPredictedTimeOfCollision(Vector3 shooterPosition, float projectileSpeed)
        {
            var velocity = this.GetOwnMovement();
            if (float.IsNaN(velocity.x) || velocity.magnitude <= 0.01f)
            {
                return null;
            }
            return TargetingCalculationHelper.GetPredictedTimeOfCollision(shooterPosition, projectileSpeed,
                this.transform.position, velocity);
        }
        
    }
}
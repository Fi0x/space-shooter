#nullable enable
using System;
using Manager;
using Ship.Weaponry;
using Ship.Weaponry.Config;
using UnityEngine;

namespace Targeting
{
    public class Targetable : MonoBehaviour
    {

        [SerializeField] private Rigidbody shipRB = null!;
        
        private void OnEnable()
        {
            if (this.shipRB == null)
            {
                this.shipRB = GetComponent<Rigidbody>() ??
                              throw new NullReferenceException("No Rigidbody set. Could not infer from GameObject.");
            }
            GameManager.Instance.TargetableManager.NotifyAboutNewTargetable(this);
        }

        private void OnDisable()
        {
            GameManager.Instance.TargetableManager.NotifyAboutTargetableGone(this);
        }
        

        public (Vector3 position, float travelTime, bool canHit)? GetPredictedTargetLocation(Vector3 shooterPosition,
            AbstractWeapon weapon)
        {
            if (weapon is HitScanWeapon hitScanWeapon)
            {
                float distanceToShooter = Vector3.Distance(shooterPosition, this.transform.position);
                
                return (this.gameObject.transform.position, 0f, (hitScanWeapon.WeaponConfig as WeaponHitScanConfigScriptableObject)!.MaxDistance > distanceToShooter);
            }
            else if (weapon is ProjectileWeapon projectileWeapon)
            {
                return this.GetPredictedTargetLocationProjectile(shooterPosition,
                    projectileWeapon.ProjectileSpeed, projectileWeapon.ProjectileTtl);
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

        private Vector3 GetOwnMovement() => this.shipRB.velocity;


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
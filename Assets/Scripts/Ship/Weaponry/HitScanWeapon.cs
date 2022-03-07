#nullable enable
using System;
using Components;
using Ship.Sensors;
using Ship.Weaponry.Config;
using UnityEngine;
using UpgradeSystem;

namespace Ship.Weaponry
{
    public class HitScanWeapon : AbstractWeapon
    {
        private Ray ray;
        private readonly RaycastHit[] raycastHits = new RaycastHit[10];

        public float MaxLength => this.weaponConfigHitScan.MaxDistance;

        [NonSerialized]
        protected WeaponHitScanConfigScriptableObject weaponConfigHitScan = null!;

        protected override void Start()
        {
            base.Start();
            this.weaponConfigHitScan = (base.weaponConfig as WeaponHitScanConfigScriptableObject) ?? 
                                throw new Exception("Provided Config cannot be applied because it is not for HitScan Weapons");
        }


        /**
         * Return a null-Tuple if no collision was detected
         */
        protected (RaycastHit hit, float distance, bool didHitEnemy)? GetRaycastHit()
        {
            var ownPosition = this.gameObject.transform.position;
            var shotDirection = this.weaponManager.Target - ownPosition;
            this.ray.origin = ownPosition;
            this.ray.direction = shotDirection;

            var size = Physics.RaycastNonAlloc(this.ray, this.raycastHits, this.weaponConfigHitScan.MaxDistance);

            if (size == 0)
            {
                return null; // No Collisions. 
            }
            
            
            // Look at first collision
            var layerMask = LayerMask.NameToLayer("Enemy");
            var firstHit = raycastHits[0];

            var collisionWithEnemy = firstHit.transform.gameObject.layer == layerMask;
            var distance = Vector3.Distance(firstHit.point, ownPosition);
            return (firstHit, distance, collisionWithEnemy);
        }
        
        
        protected override void Fire()
        {
            var raycastHitNullable = GetRaycastHit();

            if (!(raycastHitNullable is {didHitEnemy: true}))
            {
                return;
            }

            var raycastHit = raycastHitNullable.Value.hit;
            var distance = raycastHitNullable.Value.distance;

            var effectiveDamage = this.weaponConfigHitScan.DamageOverDistanceNormalized
                .Evaluate(distance / this.weaponConfigHitScan.MaxDistance) * this.upgrades[Upgrades.UpgradeNames.WeaponDamage];

            var weaponHitInformation = new WeaponHitInformation(
                WeaponHitInformation.WeaponType.HitScan,
                effectiveDamage,
                raycastHit.transform.gameObject.GetComponent<SensorTarget>()
            );

            if (raycastHit.transform.gameObject.TryGetComponent(out Health health))
            {
                // The hit "thing" can take damage
                health.TakeDamage((int) effectiveDamage);
            }
            // TODO: Spawn the Laser somehow

            this.weaponManager.EnemyHitEvent.Invoke(weaponHitInformation);
            
        }
    }
}
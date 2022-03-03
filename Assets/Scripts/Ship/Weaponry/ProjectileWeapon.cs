using System;
using Ship.Weaponry.Config;
using UnityEngine;
using UpgradeSystem;

namespace Ship.Weaponry
{
    public class ProjectileWeapon : AbstractWeapon
    {
        [NonSerialized]
        protected WeaponProjectileConfigScriptableObject weaponConfigProjectile = null!;
        
        protected override void Start()
        {
            base.Start();
            this.weaponConfigProjectile = this.weaponConfig as WeaponProjectileConfigScriptableObject ??
                                throw new Exception(
                                    "Provided Config cannot be applied because it is not for Projectile Weapons");
        }
        
        protected override void Fire()
        {
            var projectile = Instantiate(this.weaponConfigProjectile.ProjectilePrefab)!;
            var ownPosition = this.gameObject.transform.position;
            projectile.transform.position = ownPosition;
            var shotDirection = this.weaponManager.Target - ownPosition;
            var projectileDirectionAndVelocity = this.weaponConfigProjectile.ProjectileSpeed * this.shipMovementHandler.TotalMaxSpeed *
                                                 shotDirection.normalized;
            var projectileScript = projectile.GetComponent<WeaponProjectile>();
            projectileScript.Initialize(
                projectileDirectionAndVelocity, 
                this.weaponConfigProjectile.DamageOverTimeNormalized,
                this.transform.rotation, 
                this.weaponConfigProjectile.TimeToLive
            );
            projectileScript.DamageMultiplier = this.upgrades[Upgrades.UpgradeNames.WeaponDamage];
            projectileScript.WeaponHitSomethingEvent += (layer, data) =>
            {
                // Only continue if the hit target is an enemy.
                var targetLayer = LayerMask.NameToLayer("Enemy");
                if (layer == targetLayer)
                {
                    this.weaponManager.EnemyHitEvent.Invoke(data);
                }
            };
        }
    }
}
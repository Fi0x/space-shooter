using Components;
using UnityEngine;
using UnityEngine.Events;
using Upgrades;

namespace Ship
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private AnimationCurve damageOverTime;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private WeaponManager weaponManager;

        private bool isShooting;
        private float timeSinceLastFire;
        private ShipMovementHandler shipMovementHandler;
        private const float OriginalFireRate = 0.5f;
        public const float OriginalProjectileSpeedModifier = 1.5f;
        private const float OriginalProjectileDamageModifier = 1;

        private UnityAction<bool> fireModeChangedEvent;
        
        private static float FireRate => OriginalFireRate * ProjectileDamageModifier / (UpgradeStats.WeaponFireRateLevel * 0.5f);
        private static float ProjectileSpeedModifier=> OriginalProjectileSpeedModifier + UpgradeStats.ProjectileVelocityLevel * 0.1f;
        private static float ProjectileDamageModifier => OriginalProjectileDamageModifier + UpgradeStats.WeaponDamageLevel * 0.1f;

        private void Start()
        {
            this.fireModeChangedEvent += this.FireModeChangedEventHandler;
            this.weaponManager.FireModeChangedEvent.AddListener(this.fireModeChangedEvent);
            this.shipMovementHandler = this.weaponManager.GetParentShipGameObject().GetComponent<ShipMovementHandler>();

            UpgradeButton.UpgradePurchasedEvent += (sender, args) =>
            {
                switch (args.Type)
                {
                    case UpgradeButton.Upgrade.WeaponDamage:
                        UpgradeStats.WeaponDamageLevel += args.Increased ? 1 : -1;
                        break;
                    case UpgradeButton.Upgrade.WeaponFireRate:
                        UpgradeStats.WeaponFireRateLevel += args.Increased ? 1 : -1;
                        break;
                    case UpgradeButton.Upgrade.WeaponProjectileSpeed:
                        UpgradeStats.ProjectileVelocityLevel += args.Increased ? 1 : -1;
                        break;
                    default:
                        return;
                }
                
                UpgradeMenuValues.InvokeUpgradeCompletedEvent(args);
            };
        }

        private void FireModeChangedEventHandler(bool newFireMode)
        {
            this.isShooting = newFireMode;
            if (newFireMode)
            {
                this.Fire();
            }
            else
            {
                this.timeSinceLastFire = 0f;
            }
        }

        private void OnDestroy()
        {
            this.weaponManager.FireModeChangedEvent.RemoveListener(this.fireModeChangedEvent);
        }

        private void FixedUpdate()
        {
            if (this.isShooting)
            {
                if (this.timeSinceLastFire > FireRate)
                {
                    this.Fire();
                    this.timeSinceLastFire -= FireRate;
                }

                this.timeSinceLastFire += Time.fixedDeltaTime;
            }

            this.gameObject.transform.LookAt(this.weaponManager.Target, this.transform.parent.gameObject.transform.forward);
        }

        private void Fire()
        {
            var projectile = Instantiate(this.projectilePrefab);
            var ownPosition = this.gameObject.transform.position;
            projectile.transform.position = ownPosition;
            var shotDirection = this.weaponManager.Target - ownPosition;
            var projectileDirectionAndVelocity = ProjectileSpeedModifier * this.shipMovementHandler.TotalMaxSpeed * shotDirection.normalized;
            var projectileScript = projectile.GetComponent<SphereProjectile>();
            projectileScript.InitializeDirection(projectileDirectionAndVelocity, this.damageOverTime, this.transform.rotation);
            projectileScript.DamageMultiplier = OriginalProjectileDamageModifier;
            projectileScript.ProjectileHitSomethingEvent += layer =>
            {
                var targetLayer = LayerMask.NameToLayer("Enemy");
                if (layer == targetLayer)
                {
                    this.weaponManager.EnemyHitEvent.Invoke();
                }
            };
        }
    }
}
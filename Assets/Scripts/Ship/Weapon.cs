using Components;
using UI;
using UnityEngine;
using UnityEngine.Events;

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

        private UnityAction<bool> fireModeChangedEvent;
        
        public float fireRate = 0.5f;
        public float projectileSpeedModifier = 1.5f;
        public float projectileDamageModifier = 1;

        private void Start()
        {
            this.fireModeChangedEvent += this.FireModeChangedEventHandler;
            this.weaponManager.FireModeChangedEvent.AddListener(this.fireModeChangedEvent);
            this.shipMovementHandler = this.weaponManager.GetParentShipGameObject().GetComponent<ShipMovementHandler>();

            LevelTransitionMenu.UpgradePurchasedEvent += (sender, args) =>
            {
                switch (args.Type)
                {
                    case LevelTransitionMenu.Upgrade.WeaponDamage:
                        this.projectileDamageModifier += args.Increased ? 0.1f : -0.1f;
                        break;
                    case LevelTransitionMenu.Upgrade.WeaponFireRate:
                        this.fireRate *= args.Increased ? 0.5f : 2f;
                        break;
                    case LevelTransitionMenu.Upgrade.WeaponProjectileSpeed:
                        this.projectileSpeedModifier += args.Increased ? 0.1f : -0.1f;
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
                if (this.timeSinceLastFire > this.fireRate)
                {
                    this.Fire();
                    this.timeSinceLastFire -= this.fireRate;
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
            var projectileDirectionAndVelocity = this.projectileSpeedModifier * this.shipMovementHandler.TotalMaxSpeed * shotDirection.normalized;
            var projectileScript = projectile.GetComponent<SphereProjectile>();
            projectileScript.InitializeDirection(projectileDirectionAndVelocity, this.damageOverTime, this.transform.rotation);
            projectileScript.DamageMultiplier = this.projectileDamageModifier;
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
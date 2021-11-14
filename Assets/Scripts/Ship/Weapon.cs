using UnityEngine;
using UnityEngine.Events;

namespace Ship
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private AnimationCurve damageOverTime;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private WeaponManager weaponManager;

        private bool isShooting;
        private float timeSinceLastFire;

        private UnityAction<bool> fireModeChangedEvent;

        private void Start()
        {
            this.fireModeChangedEvent += this.FireModeChangedEventHandler;
            this.weaponManager.FireModeChangedEvent.AddListener(this.fireModeChangedEvent);
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
            var projectileDirectionAndVelocity = 1.2f * ShipMovementHandler.TotalMaxSpeed * shotDirection.normalized;
            var projectileScript = projectile.GetComponent<SphereProjectile>();
            projectileScript.InitializeDirection(projectileDirectionAndVelocity, LayerMask.GetMask("Enemy", "Scenery"), this.damageOverTime, this.transform.rotation);
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
#nullable enable
using System;
using System.Data;
using Components;
using Ship.Sensors;
using Ship.Weaponry.Config;
using Ship.Weaponry.Trigger;
using UnityEngine;

namespace Ship.Weaponry
{
    public class ContinuousHitScanWeapon : HitScanWeapon
    {
        protected BasicContinuousWeaponTrigger continuousWeaponTrigger = null!;

        protected ContinuousLaserAdapter laserAdapter = null!;
        
        protected override void Start()
        {
            base.SetupWeaponTrigger();
            this.continuousWeaponTrigger = base.WeaponTrigger as BasicContinuousWeaponTrigger ?? 
                                           throw new Exception("Continuous Weapon requires a Continuous Weapon Trigger, Type: "+base.WeaponTrigger.GetType());
            base.Start();
            this.InstantiateLaser();
            this.continuousWeaponTrigger.StateChangedEvent += this.HandleStateChangedEvent;
        }

        private void HandleStateChangedEvent(WeaponTriggerState state)
        {
            if (laserAdapter == null)
            {
                Debug.LogError("Cannot Update Laser because adapter is null");
                return;
            }

            this.laserAdapter.NotifyAboutLaserStateChange(state == WeaponTriggerState.Firing);
            
        }

        private void InstantiateLaser()
        {
            var prefab = this.weaponConfigHitScan.HitScanEmitterPrefab;
            var newGameObject = Instantiate(prefab, this.transform);
            // Check that required components are set.
            var hasAdapter = newGameObject.TryGetComponent<ContinuousLaserAdapter>(out this.laserAdapter);
            if (!hasAdapter)
            {
                throw new Exception("The provided prefab does not contain a Laser Adapter.");
            }
            this.laserAdapter.Init(this);
        }

        // as a fallback. should not be really used
        protected override void Fire()
        {
            this.Fire(Time.fixedDeltaTime);
        }

        protected void Fire(float timeBetweenLastFrameInMillis)
        {
            var rayHitNullable = base.GetRaycastHit();
            if (rayHitNullable == null)
            {
                this.laserAdapter.NotifyAboutLaserNotHittingTarget();
                return;
            }
            
            

            var hit = rayHitNullable.Value.hit;
            var distance = rayHitNullable.Value.distance;
            var hitEnemy = rayHitNullable.Value.didHitEnemy;
            this.laserAdapter.NotifyAboutLaserHittingTarget(distance);

            if (!hitEnemy)
            {
                return;
            }
            var effectiveDamage = this.weaponConfigHitScan.DamageOverDistanceNormalized
                .Evaluate(distance / this.weaponConfigHitScan.MaxDistance) * timeBetweenLastFrameInMillis;

            var weaponHitInformation = new WeaponHitInformation(
                WeaponHitInformation.WeaponType.HitScan,
                effectiveDamage,
                hit.transform.gameObject.GetComponent<SensorTarget>()
            );

            if (hit.transform.gameObject.TryGetComponent(out Health health))
            {
                // The hit "thing" can take damage
                health.TakeDamage(effectiveDamage);
            }
            
            this.weaponManager.EnemyHitEvent.Invoke(weaponHitInformation);

        }

        protected override void SubscribeToWeaponTrigger()
        {
            this.continuousWeaponTrigger.WeaponFiredEventWithDeltaTime += Fire;
        }
    }
}
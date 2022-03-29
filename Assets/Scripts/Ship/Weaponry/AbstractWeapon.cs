#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Ship.Movement;
using Ship.Weaponry.Config;
using Ship.Weaponry.Trigger;
using UI.Upgrade;
using UnityEngine;
using UpgradeSystem;

namespace Ship.Weaponry
{
    public abstract class AbstractWeapon : MonoBehaviour, IUpgradeable
    {
        [SerializeField] protected WeaponManager weaponManager = null!;
        [SerializeField] protected WeaponConfigScriptableObject weaponConfig = null!;

        public IWeaponTrigger WeaponTrigger { get; protected set; } = null!;
        [SerializeField] protected UpgradeDataSO upgradeData;
        
        protected PlayerShipMovementHandler playerShipMovementHandler = null!;

        public WeaponConfigScriptableObject WeaponConfig => this.weaponConfig;
        
        public abstract bool IsHitScan { get; }
        
        private void OnEnable()
        {
            if (this.weaponManager == null)
            {
                this.weaponManager = this.gameObject.GetComponentInParent<WeaponAttachmentPoint>().WeaponManager;
            }
            this.weaponManager.FireModeChangedEvent += this.FireModeChangedEventHandler;
        }

        private void OnDisable()
        {
            this.weaponManager.FireModeChangedEvent -= this.FireModeChangedEventHandler;
        }
        
        protected virtual void SetupWeaponTrigger()
        {
            this.WeaponTrigger ??= this.weaponConfig.BuildWeaponTrigger() ?? throw new NullReferenceException();
        }

        protected virtual void SubscribeToWeaponTrigger()
        {
            if (this.WeaponTrigger == null)
            {
                throw new Exception("Weapon Trigger is not registered");
            }
            this.WeaponTrigger.WeaponFiredEvent += Fire;
        }

        protected virtual void Start()
        {
            this.ResetUpgrades();
            
            _ = (object)this.weaponConfig ?? throw new NullReferenceException("No Weapon Config is set");
            
            this.SetupWeaponTrigger();
            this.SubscribeToWeaponTrigger();
            
            this.playerShipMovementHandler = this.weaponManager.GetParentShipGameObject().GetComponent<PlayerShipMovementHandler>() ?? throw new NullReferenceException();

            UpgradeButton.UpgradePurchasedEvent += (sender, args) =>
            {
                if (this.WeaponTrigger == null)
                {
                    throw new Exception("Weapon trigger is null");
                }
                
                if (args.Type.Equals(Upgrades.UpgradeNames.WeaponFireRate))
                    this.WeaponTrigger.ShotDelayUpgradeLevel = UpgradeHandler.GetSpecificUpgrade(Upgrades.UpgradeNames.WeaponFireRate);
            };
        }

        private void FireModeChangedEventHandler(bool isFiring)
        {
            this.WeaponTrigger?.NotifyAboutTriggerStateChange(isFiring);
        }

        private void FixedUpdate()
        {
            this.gameObject.transform.LookAt(this.weaponManager.Target, this.transform.parent.gameObject.transform.forward);
            this.WeaponTrigger?.Update(Time.fixedDeltaTime);
        }
    
        protected abstract void Fire();

        public virtual void Remove()
        {
            this.WeaponTrigger = null; // Not sure if needed :)
            Destroy(this.gameObject);
        }

        public void ResetUpgrades()
        {
            // this.upgrades.Clear();
            //
            // this.upgrades.Add(Upgrades.UpgradeNames.WeaponFireRate, 1);
            // this.upgrades.Add(Upgrades.UpgradeNames.WeaponProjectileSpeed, 1);
            // this.upgrades.Add(Upgrades.UpgradeNames.WeaponDamage, 1);
            //
            // UpgradeHandler.RegisterUpgrades(this, this.upgrades.Keys.ToList());
        }

        public void SetNewUpgradeValue(Enum type, int newLevel)
        {
            // if (this.upgrades.ContainsKey(type))
            //     this.upgrades[type] = newLevel;
        }
    }
}
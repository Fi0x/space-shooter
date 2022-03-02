#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Ship.Weaponry.Config;
using Ship.Weaponry.Trigger;
using UnityEngine;
using Upgrades;

namespace Ship.Weaponry
{
    public abstract class AbstractWeapon : MonoBehaviour, IUpgradeable
    {
        [SerializeField] protected WeaponManager weaponManager = null!;
        [SerializeField] protected WeaponConfigScriptableObject weaponConfig = null!;

        public IWeaponTrigger WeaponTrigger { get; protected set; } = null!;
        //TODO: Use for upgrades
        private readonly Dictionary<Enum, int> upgrades = new Dictionary<Enum, int>();
        
        protected ShipMovementHandler shipMovementHandler = null!;
        
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
            this.WeaponTrigger.WeaponFiredEvent += Fire;
        }

        protected virtual void Start()
        {
            //TODO: Do this in all child-classes
            this.ResetUpgrades();
            
            _ = (object)this.weaponConfig ?? throw new NullReferenceException("No Weapon Config is set");
            
            this.SetupWeaponTrigger();
            this.SubscribeToWeaponTrigger();
            
            this.shipMovementHandler = this.weaponManager.GetParentShipGameObject().GetComponent<ShipMovementHandler>() ?? throw new NullReferenceException();
        }

        private void FireModeChangedEventHandler(bool isFiring)
        {
            this.WeaponTrigger.NotifyAboutTriggerStateChange(isFiring);
        }

        private void FixedUpdate()
        {
            this.gameObject.transform.LookAt(this.weaponManager.Target, this.transform.parent.gameObject.transform.forward);
            this.WeaponTrigger.Update(Time.fixedDeltaTime);
        }
    
        protected abstract void Fire();

        public virtual void Remove()
        {
            this.WeaponTrigger = null; // Not sure if needed :)
            Destroy(this.gameObject);
        }

        public void ResetUpgrades()
        {
            this.upgrades.Clear();
            
            //TODO: Add all upgrade types
            this.upgrades.Add(UI.Upgrade.Upgrades.UpgradeNames.WeaponDamage, 1);
            
            UpgradeHandler.RegisterUpgrades(this, this.upgrades.Keys.ToList());
        }

        public void SetNewUpgradeValue(Enum type, int newLevel)
        {
            //TODO: Check all cases
            if (this.upgrades.ContainsKey(type))
                this.upgrades[type] = newLevel;
        }
    }
}
using System;
using Ship.Weaponry.Config;
using Ship.Weaponry.Trigger;
using UI;
using UnityEngine;

namespace Ship.Weaponry
{
    public abstract class AbstractWeapon : MonoBehaviour
    {
        [SerializeField] protected WeaponManager weaponManager = null!;
        [SerializeField] protected WeaponConfigScriptableObject weaponConfig = null!;

        public IWeaponTrigger WeaponTrigger { get; protected set; } = null!;
        
        protected ShipMovementHandler shipMovementHandler = null!;
        
        private void OnEnable()
        {
            this.weaponManager.FireModeChangedEvent += this.FireModeChangedEventHandler;
        }

        private void OnDisable()
        {
            this.weaponManager.FireModeChangedEvent -= this.FireModeChangedEventHandler;
        }

        protected virtual void Start()
        {
            _ = (object)this.weaponConfig ?? throw new NullReferenceException("No Weapon Config is set");
            this.WeaponTrigger = this.weaponConfig.BuildWeaponTrigger() ?? throw new NullReferenceException();
            this.WeaponTrigger.WeaponFiredEvent += Fire;
            
            this.shipMovementHandler = this.weaponManager.GetParentShipGameObject().GetComponent<ShipMovementHandler>() ?? throw new NullReferenceException();

            LevelTransitionMenu.UpgradePurchasedEvent += (sender, args) =>
            {
                /* TODO 
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
                */
                
                UpgradeMenuValues.InvokeUpgradeCompletedEvent(args);
            };
        }

        private void FireModeChangedEventHandler(bool isFiring)
        {
            Debug.Log("Changed mode to "+isFiring);
            this.WeaponTrigger.NotifyAboutTriggerStateChange(isFiring);
        }

        private void FixedUpdate()
        {
            this.gameObject.transform.LookAt(this.weaponManager.Target, this.transform.parent.gameObject.transform.forward);
            this.WeaponTrigger.Update(Time.fixedDeltaTime);
        }

        protected abstract void Fire();
    }
}
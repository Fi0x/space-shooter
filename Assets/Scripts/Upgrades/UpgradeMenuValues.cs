using System;
using Components;
using Manager;
using Ship;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    public class UpgradeMenuValues : MonoBehaviour
    {
        private Text text;
        private OldUpgradeButton.Upgrade type;

        public static event EventHandler<OldUpgradeButton.UpgradePurchasedEventArgs> UpgradeCompletedEvent;

        public static void InvokeUpgradeCompletedEvent(OldUpgradeButton.UpgradePurchasedEventArgs args)
        {
            UpgradeCompletedEvent?.Invoke(null, args);
        }
        
        private void Start()
        {
            this.text = this.gameObject.GetComponent<Text>();
            this.type = this.GetUpgradeType();
            
            this.text.text = this.GetCorrectValue();

            UpgradeCompletedEvent += this.UpdateValue;
        }

        private void UpdateValue(object sender, OldUpgradeButton.UpgradePurchasedEventArgs args)
        {
            if(args.Type != this.type)
                return;
            this.text.text = this.GetCorrectValue();
        }

        private string GetCorrectValue()
        {
            switch (this.type)
            {
                    case OldUpgradeButton.Upgrade.WeaponDamage:
                        return "" + UpgradeStats.WeaponDamageLevel;
                        
                    case OldUpgradeButton.Upgrade.WeaponFireRate:
                        return "" + UpgradeStats.WeaponFireRateLevel;
                    
                    case OldUpgradeButton.Upgrade.WeaponProjectileSpeed:
                        return "" + UpgradeStats.ProjectileVelocityLevel;
                    
                    case OldUpgradeButton.Upgrade.EngineAcceleration:
                        return "" + UpgradeStats.ShipAccelerationLevel;
                        
                    case OldUpgradeButton.Upgrade.EngineDeceleration:
                        return "" + UpgradeStats.ShipBrakeLevel;
                        
                    case OldUpgradeButton.Upgrade.EngineLateralThrust:
                        return "" + UpgradeStats.ShipLateralThrustLevel;
                        
                    case OldUpgradeButton.Upgrade.EngineRotationSpeedPitch:
                        return "" + UpgradeStats.ShipPitchSpeedLevel;
                        
                    case OldUpgradeButton.Upgrade.EngineRotationSpeedRoll:
                        return "" + UpgradeStats.ShipRollSpeedLevel;
                        
                    case OldUpgradeButton.Upgrade.EngineRotationSpeedYaw:
                        return "" + UpgradeStats.ShipYawSpeedLevel;
                        
                    case OldUpgradeButton.Upgrade.EngineStabilizationSpeed:
                        return "" + UpgradeStats.ShipStabilizerLevel;
                    
                    case OldUpgradeButton.Upgrade.Unknown:
                        return "?";
                    
                    default:
                        return "1";
            }
        }

        private OldUpgradeButton.Upgrade GetUpgradeType() => this.gameObject.name switch
        {
            "WpnDmgValue" => OldUpgradeButton.Upgrade.WeaponDamage,
            "WpnRateValue" => OldUpgradeButton.Upgrade.WeaponFireRate,
            "WpnProjVelValue" => OldUpgradeButton.Upgrade.WeaponProjectileSpeed,
            "EngAccValue" => OldUpgradeButton.Upgrade.EngineAcceleration,
            "EngDecValue" => OldUpgradeButton.Upgrade.EngineDeceleration,
            "EngLatValue" => OldUpgradeButton.Upgrade.EngineLateralThrust,
            "EngRotPitchValue" => OldUpgradeButton.Upgrade.EngineRotationSpeedPitch,
            "EngRotRollValue" => OldUpgradeButton.Upgrade.EngineRotationSpeedRoll,
            "EngRotYawValue" => OldUpgradeButton.Upgrade.EngineRotationSpeedYaw,
            "EngStabValue" => OldUpgradeButton.Upgrade.EngineStabilizationSpeed,
            _ => OldUpgradeButton.Upgrade.Unknown
        };
    }
}
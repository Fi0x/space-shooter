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
        private UpgradeButton.Upgrade type;

        public static event EventHandler<UpgradeButton.UpgradePurchasedEventArgs> UpgradeCompletedEvent;

        public static void InvokeUpgradeCompletedEvent(UpgradeButton.UpgradePurchasedEventArgs args)
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

        private void UpdateValue(object sender, UpgradeButton.UpgradePurchasedEventArgs args)
        {
            if(args.Type != this.type)
                return;
            this.text.text = this.GetCorrectValue();
        }

        private string GetCorrectValue()
        {
            switch (this.type)
            {
                    case UpgradeButton.Upgrade.WeaponDamage:
                        return "" + UpgradeStats.WeaponDamageLevel;
                        
                    case UpgradeButton.Upgrade.WeaponFireRate:
                        return "" + UpgradeStats.WeaponFireRateLevel;
                    
                    case UpgradeButton.Upgrade.WeaponProjectileSpeed:
                        return "" + UpgradeStats.ProjectileVelocityLevel;
                    
                    case UpgradeButton.Upgrade.EngineAcceleration:
                        return "" + UpgradeStats.ShipAccelerationLevel;
                        
                    case UpgradeButton.Upgrade.EngineDeceleration:
                        return "" + UpgradeStats.ShipBrakeLevel;
                        
                    case UpgradeButton.Upgrade.EngineLateralThrust:
                        return "" + UpgradeStats.ShipLateralThrustLevel;
                        
                    case UpgradeButton.Upgrade.EngineRotationSpeedPitch:
                        return "" + UpgradeStats.ShipPitchSpeedLevel;
                        
                    case UpgradeButton.Upgrade.EngineRotationSpeedRoll:
                        return "" + UpgradeStats.ShipRollSpeedLevel;
                        
                    case UpgradeButton.Upgrade.EngineRotationSpeedYaw:
                        return "" + UpgradeStats.ShipYawSpeedLevel;
                        
                    case UpgradeButton.Upgrade.EngineStabilizationSpeed:
                        return "" + UpgradeStats.ShipStabilizerLevel;
                        
                    case UpgradeButton.Upgrade.Armor:
                        return "" + UpgradeStats.ArmorLevel;
                    
                    case UpgradeButton.Upgrade.Unknown:
                        return "?";
                    
                    default:
                        return "1";
            }
        }

        private UpgradeButton.Upgrade GetUpgradeType() => this.gameObject.name switch
        {
            "WpnDmgValue" => UpgradeButton.Upgrade.WeaponDamage,
            "WpnRateValue" => UpgradeButton.Upgrade.WeaponFireRate,
            "WpnProjVelValue" => UpgradeButton.Upgrade.WeaponProjectileSpeed,
            "EngAccValue" => UpgradeButton.Upgrade.EngineAcceleration,
            "EngDecValue" => UpgradeButton.Upgrade.EngineDeceleration,
            "EngLatValue" => UpgradeButton.Upgrade.EngineLateralThrust,
            "EngRotPitchValue" => UpgradeButton.Upgrade.EngineRotationSpeedPitch,
            "EngRotRollValue" => UpgradeButton.Upgrade.EngineRotationSpeedRoll,
            "EngRotYawValue" => UpgradeButton.Upgrade.EngineRotationSpeedYaw,
            "EngStabValue" => UpgradeButton.Upgrade.EngineStabilizationSpeed,
            "HPValue" => UpgradeButton.Upgrade.Armor,
            _ => UpgradeButton.Upgrade.Unknown
        };
    }
}
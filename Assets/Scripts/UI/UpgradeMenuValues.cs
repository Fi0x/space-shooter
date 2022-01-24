using System;
using Components;
using Manager;
using Ship;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UpgradeMenuValues : MonoBehaviour
    {
        private Text text;
        private LevelTransitionMenu.Upgrade type;

        public static event EventHandler<LevelTransitionMenu.UpgradePurchasedEventArgs> UpgradeCompletedEvent;

        public static void InvokeUpgradeCompletedEvent(LevelTransitionMenu.UpgradePurchasedEventArgs args)
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

        private void UpdateValue(object sender, LevelTransitionMenu.UpgradePurchasedEventArgs args)
        {
            if(args.Type != this.type)
                return;
            this.text.text = this.GetCorrectValue();
        }

        private string GetCorrectValue()
        {
            switch (this.type)
            {
                    case LevelTransitionMenu.Upgrade.WeaponDamage:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponentInChildren<Weapon>().projectileDamageModifier, 2);
                        
                    case LevelTransitionMenu.Upgrade.WeaponFireRate:
                        return "" + 1f / Math.Round(GameManager.Instance.Player.GetComponentInChildren<Weapon>().fireRate, 2);
                    
                    case LevelTransitionMenu.Upgrade.WeaponProjectileSpeed:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponentInChildren<Weapon>().projectileSpeedModifier, 2);
                    
                    case LevelTransitionMenu.Upgrade.EngineAcceleration:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.AccelerationForwards, 2);
                        
                    case LevelTransitionMenu.Upgrade.EngineDeceleration:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.AccelerationBackwards, 2);
                        
                    case LevelTransitionMenu.Upgrade.EngineLateralThrust:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.AccelerationLateral, 2);
                        
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedPitch:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.PitchSpeed, 2);
                        
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedRoll:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.RollSpeed, 2);
                        
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedYaw:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.YawSpeed, 2);
                        
                    case LevelTransitionMenu.Upgrade.EngineStabilizationSpeed:
                        return "" + Math.Round(GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.StabilizationMultiplier, 2);
                        
                    case LevelTransitionMenu.Upgrade.Armor:
                        var playerHp = GameManager.Instance.Player.GetComponent<Health>();
                        return playerHp.MaxHealth.ToString();
                    
                    case LevelTransitionMenu.Upgrade.Unknown:
                        return "?";
                    
                    default:
                        return "0";
            }
        }

        private LevelTransitionMenu.Upgrade GetUpgradeType() => this.gameObject.name switch
        {
            "WpnDmgValue" => LevelTransitionMenu.Upgrade.WeaponDamage,
            "WpnRateValue" => LevelTransitionMenu.Upgrade.WeaponFireRate,
            "WpnProjVelValue" => LevelTransitionMenu.Upgrade.WeaponProjectileSpeed,
            "EngAccValue" => LevelTransitionMenu.Upgrade.EngineAcceleration,
            "EngDecValue" => LevelTransitionMenu.Upgrade.EngineDeceleration,
            "EngLatValue" => LevelTransitionMenu.Upgrade.EngineLateralThrust,
            "EngRotPitchValue" => LevelTransitionMenu.Upgrade.EngingRotationSpeedPitch,
            "EngRotRollValue" => LevelTransitionMenu.Upgrade.EngingRotationSpeedRoll,
            "EngRotYawValue" => LevelTransitionMenu.Upgrade.EngingRotationSpeedYaw,
            "EngStabValue" => LevelTransitionMenu.Upgrade.EngineStabilizationSpeed,
            "HPValue" => LevelTransitionMenu.Upgrade.Armor,
            _ => LevelTransitionMenu.Upgrade.Unknown
        };
    }
}
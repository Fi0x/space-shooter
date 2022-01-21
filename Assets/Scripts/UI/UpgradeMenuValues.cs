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
        private void Start()
        {
            this.text = this.gameObject.GetComponent<Text>();
            this.type = this.GetUpgradeType();
            
            this.text.text = this.GetCorrectValue();

            LevelTransitionMenu.UpgradePurchasedEvent += this.UpdateValue;
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
                        return "" + 1f / GameManager.Instance.Player.GetComponentInChildren<Weapon>().projectileDamageModifier;
                        
                    case LevelTransitionMenu.Upgrade.WeaponFireRate:
                        return "" + 1f / GameManager.Instance.Player.GetComponentInChildren<Weapon>().fireRate;
                    
                    case LevelTransitionMenu.Upgrade.WeaponProjectileSpeed:
                        return "" + GameManager.Instance.Player.GetComponentInChildren<Weapon>().projectileSpeedModifier;
                    
                    case LevelTransitionMenu.Upgrade.EngineAcceleration:
                        return "" + GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.AccelerationForwards;
                        
                    case LevelTransitionMenu.Upgrade.EngineDeceleration:
                        return "" + GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.AccelerationBackwards;
                        
                    case LevelTransitionMenu.Upgrade.EngineLateralThrust:
                        return "" + GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.AccelerationLateral;
                        
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedPitch:
                        return "" + GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.PitchSpeed;
                        
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedRoll:
                        return "" + GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.RollSpeed;
                        
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedYaw:
                        return "" + GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.YawSpeed;
                        
                    case LevelTransitionMenu.Upgrade.EngineStabilizationSpeed:
                        return "" + GameManager.Instance.Player.GetComponent<ShipMovementHandler>().Settings.StabilizationMultiplier;
                        
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
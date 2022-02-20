using System;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private GameObject valueObject;
        
        private bool isIncrement;
        private Button button;
        private Upgrade type;

        public static event EventHandler<UpgradePurchasedEventArgs> UpgradePurchasedEvent;
        
        private void Start()
        {
            this.isIncrement = this.gameObject.name.Equals("Increase");
            this.type = this.GetUpgradeType();
            this.button = this.GetComponent<Button>();
            
            this.UpdateVisibility();

            UpgradeMenuValues.UpgradeCompletedEvent += (sender, args) =>
            {
                this.UpdateVisibility();
            };
        }

        public void UpgradeButtonPressed()
        {
            UpgradeStats.FreeUpgradePoints += this.isIncrement ? -1 : 1;
            UpgradePurchasedEvent?.Invoke(null, new UpgradePurchasedEventArgs(this.type, this.isIncrement));

            StatCollector.IntStats[nameof(StatCollector.StatValues.UpgradesPurchased)] += this.isIncrement ? 1 : -1;
        }

        private void UpdateVisibility()
        {
            if (this.isIncrement)
                this.button.interactable = UpgradeStats.FreeUpgradePoints > 0;
            else
                this.button.interactable = UpgradeStats.GetCurrentLevel(this.type) > 1;
        }

        private Upgrade GetUpgradeType() => this.valueObject.name switch
        {
            "WpnDmgValue" => Upgrade.WeaponDamage,
            "WpnRateValue" => Upgrade.WeaponFireRate,
            "WpnProjVelValue" => Upgrade.WeaponProjectileSpeed,
            "EngAccValue" => Upgrade.EngineAcceleration,
            "EngDecValue" => Upgrade.EngineDeceleration,
            "EngLatValue" => Upgrade.EngineLateralThrust,
            "EngRotPitchValue" => Upgrade.EngineRotationSpeedPitch,
            "EngRotRollValue" => Upgrade.EngineRotationSpeedRoll,
            "EngRotYawValue" => Upgrade.EngineRotationSpeedYaw,
            "EngStabValue" => Upgrade.EngineStabilizationSpeed,
            "HPValue" => Upgrade.Armor,
            _ => Upgrade.Unknown
        };
        
        

        public class UpgradePurchasedEventArgs : EventArgs
        {
            public readonly Upgrade Type;
            public readonly bool Increased;
        
            public UpgradePurchasedEventArgs(Upgrade upgradeType, bool increased)
            {
                this.Type = upgradeType;
                this.Increased = increased;
            }
        }
        
        public enum Upgrade
        {
            WeaponDamage,
            WeaponFireRate,
            WeaponProjectileSpeed,
            EngineAcceleration,
            EngineDeceleration,
            EngineLateralThrust,
            EngineRotationSpeedPitch,
            EngineRotationSpeedRoll,
            EngineRotationSpeedYaw,
            EngineStabilizationSpeed,
            Armor,
            Unknown
        }
    }
}
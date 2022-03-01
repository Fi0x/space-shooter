using System;
using System.Collections.Generic;
using System.Linq;

namespace Upgrades
{
    public static class UpgradeHandler
    {
        private static readonly List<IUpgradeable> UpgradeClasses = new List<IUpgradeable>();
        private static readonly Dictionary<Enum, int> Upgrades = new Dictionary<Enum, int>();

        public static event EventHandler<UpgradePurchasedEventArgs> UpgradePurchasedEvent;

        //TODO: Use instead of old UpgradeStats
        public static void RegisterUpgrades(IUpgradeable upgradeClass, List<Enum> upgradeList)
        {
            if (!UpgradeClasses.Contains(upgradeClass))
                UpgradeClasses.Add(upgradeClass);
            
            foreach (var upgrade in upgradeList)
            {
                if(Upgrades.ContainsKey(upgrade))
                    continue;
                Upgrades.Add(upgrade, 1);
            }
        }

        //TODO: Use in new upgrade-screen
        public static Dictionary<Enum, int> GetAllUpgrades() => Upgrades;

        //TODO: Use in new upgrade-screen
        public static void UpdateUpgrade(Enum upgradeName, int newValue)
        {
            UpgradePurchasedEvent?.Invoke(null, new UpgradePurchasedEventArgs(upgradeName, newValue));
        }
        
        public static void Reset()
        {
            UpgradeClasses.ForEach(c => c.ResetUpgrades());
        }
        
        public class UpgradePurchasedEventArgs : EventArgs
        {
            public readonly Enum Name;
            public readonly int NewValue;
        
            public UpgradePurchasedEventArgs(Enum upgradeName, int newValue)
            {
                this.Name = upgradeName;
                this.NewValue = newValue;
            }
        }
        
        public enum UpgradeNames
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
            
            Health
        }
    }
}
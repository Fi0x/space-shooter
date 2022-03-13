using System;
using System.Collections.Generic;
using System.Linq;

namespace UpgradeSystem
{
    public static class UpgradeHandler
    {
        public static int FreeUpgradePoints;

        public static event EventHandler NewUpgradeRegistered;
        
        private static readonly List<IUpgradeable> UpgradeClasses = new List<IUpgradeable>();
        private static readonly Dictionary<Enum, int> Upgrades = new Dictionary<Enum, int>();

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
            
            NewUpgradeRegistered?.Invoke(null, null);
        }

        public static Dictionary<Enum, int> GetAllUpgrades() => Upgrades;
        public static int GetSpecificUpgrade(Enum upgradeType) => Upgrades.First(e => e.Key.Equals(upgradeType)).Value;

        public static string GetUpgradeCategory(Enum upgradeType) => (Upgrades.UpgradeNames) upgradeType switch
        {
            UpgradeSystem.Upgrades.UpgradeNames.WeaponDamage => "Weapons",
            UpgradeSystem.Upgrades.UpgradeNames.WeaponFireRate => "Weapons",
            UpgradeSystem.Upgrades.UpgradeNames.WeaponProjectileSpeed => "Weapons",

            UpgradeSystem.Upgrades.UpgradeNames.EngineAcceleration => "Movement",
            UpgradeSystem.Upgrades.UpgradeNames.EngineDeceleration => "Movement",
            UpgradeSystem.Upgrades.UpgradeNames.EngineLateralThrust => "Movement",
            UpgradeSystem.Upgrades.UpgradeNames.EngineRotationSpeedPitch => "Movement",
            UpgradeSystem.Upgrades.UpgradeNames.EngineRotationSpeedRoll => "Movement",
            UpgradeSystem.Upgrades.UpgradeNames.EngineRotationSpeedYaw => "Movement",
            UpgradeSystem.Upgrades.UpgradeNames.EngineStabilizationSpeed => "Movement",

            UpgradeSystem.Upgrades.UpgradeNames.Health => "Health",

            _ => "Unknown",
        };

        public static void PurchaseUpgrade(Enum upgradeName, int valueChange)
        {
            FreeUpgradePoints -= valueChange;
            Upgrades[upgradeName] += valueChange;

            foreach (var upgradeClass in UpgradeClasses)
                upgradeClass.SetNewUpgradeValue(upgradeName, Upgrades[upgradeName]);
        }
        
        public static void Reset()
        {
            FreeUpgradePoints = 0;
            UpgradeClasses.ForEach(c => c.ResetUpgrades());
        }
    }
}
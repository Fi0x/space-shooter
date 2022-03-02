using System;
using System.Collections.Generic;
using System.Linq;

namespace Upgrades
{
    public static class UpgradeHandler
    {
        public static int FreeUpgradePoints;
        
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
        }

        public static Dictionary<Enum, int> GetAllUpgrades() => Upgrades;
        public static int GetSpecificUpgrade(Enum upgradeType) => Upgrades.First(e => e.Key.Equals(upgradeType)).Value;

        public static void PurchaseUpgrade(Enum upgradeName, int valueChange)
        {
            FreeUpgradePoints -= valueChange;
            Upgrades[upgradeName] += valueChange;
        }
        
        public static void Reset()
        {
            UpgradeClasses.ForEach(c => c.ResetUpgrades());
        }
    }
}
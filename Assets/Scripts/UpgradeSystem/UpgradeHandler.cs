using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
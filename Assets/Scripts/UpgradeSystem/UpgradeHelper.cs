using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    public static class UpgradeHelper
    {
        public static string GetUpgradeCategory(UpgradeNames upgradeType) => upgradeType switch
        {
            UpgradeNames.WeaponDamage => "Weapons",
            UpgradeNames.WeaponFireRate => "Weapons",
            UpgradeNames.WeaponProjectileSpeed => "Weapons",

            UpgradeNames.EngineAcceleration => "Movement",
            UpgradeNames.EngineStabilizationSpeed => "Movement",
            
            UpgradeNames.RocketChargeSpeed => "Rocket",
            UpgradeNames.MaxRockets => "Rocket",

            UpgradeNames.Health => "Health",

            _ => "Unknown",
        };
        
        public static List<T> FisherYatesCardDeckShuffle<T>(List<T>aList) {
 
            System.Random _random = new System.Random ();
 
            T myGO;
 
            int n = aList.Count;
            for (int i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + (int)(_random.NextDouble() * (n - i));
                myGO = aList[r];
                aList[r] = aList[i];
                aList[i] = myGO;
            }
 
            return aList;
        }
    }
}
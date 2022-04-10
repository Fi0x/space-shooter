using System;
using System.Text.RegularExpressions;

namespace UpgradeSystem
{
    [Serializable]
    public class Upgrade
    {
        public UpgradeNames type;
        public float costMultiplier = 1f;
        public int points;

        public Upgrade()
        {
            type = UpgradeNames.Health;
            points = 1;
        }
        
        public Upgrade(UpgradeNames type, int points)
        {
            this.type = type;
            this.points = points;
        }
        
        public static string GetDisplayName(Enum type)
        {
            return Regex.Replace(type.ToString(), "(\\B[A-Z])", " $1");
        }

        public static UpgradeNames GetTypeFromDisplayName(string displayName)
        {
            return (UpgradeNames) Enum.Parse(typeof(UpgradeNames), displayName.Replace(" ", ""));
        }
    }
    
    public enum UpgradeNames
    {
        WeaponType,
        WeaponDamage,
        WeaponFireRate,
        WeaponProjectileSpeed,
            
        EngineAcceleration,
        EngineHandling,
        EngineStabilizationSpeed,
            
        Health,
        
        MaxRockets,
        RocketChargeSpeed,
    }
}
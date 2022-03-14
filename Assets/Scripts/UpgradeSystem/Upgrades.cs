using System;
using System.Text.RegularExpressions;

namespace UpgradeSystem
{
    public static class Upgrades
    {
        public static string GetDisplayName(Enum type)
        {
            return Regex.Replace(type.ToString(), "(\\B[A-Z])", " $1");
        }

        public static UpgradeNames GetTypeFromDisplayName(string displayName)
        {
            return (UpgradeNames) Enum.Parse(typeof(UpgradeNames), displayName.Replace(" ", ""));
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
            
            Health,
            
            Unknown
        }
    }
}
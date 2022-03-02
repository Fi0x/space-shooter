using System;
using System.Collections.Generic;

namespace UI.Upgrade
{
    public static class Upgrades
    {
        public static string GetDisplayName(Enum type)
        {
            var upperLetters = new List<int>();

            var typeString = type.ToString();
            for (var i = 0; i < typeString.Length; i++)
            {
                if(Char.IsUpper(typeString[i]))
                    upperLetters.Add(i);
            }
            for (var i = upperLetters.Count - 1; i > 0; i--)
                typeString = typeString.Insert(i, " ");

            return typeString;
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
            
            Health
        }
    }
}
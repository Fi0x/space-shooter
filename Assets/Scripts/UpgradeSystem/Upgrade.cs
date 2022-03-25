using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UpgradeSystem
{
    [Serializable]
    public class Upgrade
    {
        public UpgradeNames type;
        public CalculationType calculationType;
        public float costMultiplier = 1f;
        public int points;

        public Upgrade()
        {
            type = UpgradeNames.Unknown;
            calculationType = CalculationType.Linear;
            points = 1;
        }
        
        public Upgrade(UpgradeNames type, CalculationType calcType, int points)
        {
            this.type = type;
            this.calculationType = calcType;
            this.points = points;
        }
        
        public float GetValue() => calculationType switch
        {
            CalculationType.Linear => CalculateLinear(),
            CalculationType.Exponential => CalculateExponential(),
            CalculationType.Logarithmic => CalculateLogarithmic(),
            _ => 0
        };

        private float CalculateLinear()
        {
            return points;
        }

        private float CalculateExponential()
        {
            return Mathf.Exp(points);
        }

        private float CalculateLogarithmic()
        {
            return Mathf.Log(points + 1)/Mathf.Log(2);
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
        
        MaxRockets,
        RocketChargeSpeed,
            
        Unknown
    }

    public enum CalculationType
    {
        Linear,
        Logarithmic,
        Exponential
    }
}
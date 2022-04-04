using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem.CostAndGain
{
    [CreateAssetMenu(fileName = "DefaultUpgradeSystemScriptableObject", menuName = "ScriptableObject/Gameplay/Upgrading/Default",
        order = 50)]
    public class DefaultUpgradeSystemCostAndGainLookupScriptableObjectImplementation : UpgradeSystemCostAndGainLookupScriptableObject
    {
        protected override Dictionary<UpgradeNames, uint> MaxLevel { get; } = new Dictionary<UpgradeNames, uint>
        {
            {UpgradeNames.Health, 50},
            {UpgradeNames.MaxRockets, 10},
            {UpgradeNames.RocketChargeSpeed, 10},
            {UpgradeNames.WeaponDamage, 20},
            {UpgradeNames.WeaponProjectileSpeed, 20},
            {UpgradeNames.WeaponFireRate, 3},
            {UpgradeNames.EngineAcceleration, 20},
            {UpgradeNames.EngineHandling, 20},
            {UpgradeNames.EngineStabilizationSpeed, 20},
        };
        
        protected override bool CanUpgradeImpl(UpgradeNames upgrade, int level)
        {
            return this.MaxLevel[upgrade] > level;
        }

        private void ValidateAndThrowOnUpgradeOutOfBounds(UpgradeNames upgrade, int level)
        {
            return;
            if (!CanUpgradeImpl(upgrade, level))
            {
                throw new ArgumentOutOfRangeException(nameof(level),
                    "Level cannot be greater than " + this.MaxLevel[upgrade]);
            }
        }

        protected override UpgradeData GetUpgradeDataForLevelImpl(UpgradeNames upgrade, int level)
        {
            switch (upgrade)
            {
                case UpgradeNames.Health:
                    return this.GetUpgradeDataForLevelHealth(level);
                
                // Rockets
                
                case UpgradeNames.MaxRockets:
                    return this.GetUpgradeDataForMaxRockets(level);
                case UpgradeNames.RocketChargeSpeed:
                    return this.GetUpgradeDataForRocketChargeSpeed(level);
                
                // Weapons
                
                case UpgradeNames.WeaponDamage:
                    float WeaponDamageFn(int lvl) => (float) (1 + (.8f * lvl - 0.02f * lvl * lvl) * .5);
                    return this.GetUpgradeDataForWeaponMultiplier(level, WeaponDamageFn, UpgradeNames.WeaponDamage);
                case UpgradeNames.WeaponProjectileSpeed:
                    float WeaponProjSpeedFn(int lvl) => (float) (1 + (.8f * lvl - 0.02f * lvl * lvl) * .2);
                    return this.GetUpgradeDataForWeaponMultiplier(level, WeaponProjSpeedFn, UpgradeNames.WeaponProjectileSpeed);
                case UpgradeNames.WeaponFireRate:
                    float WeaponTimeBetweenShotMultiplierFn(int lvl) => (float) (1 - (.8f * lvl - 0.04f * lvl * lvl) / 8);
                    return this.GetUpgradeDataForWeaponMultiplier(level, WeaponTimeBetweenShotMultiplierFn, UpgradeNames.WeaponFireRate);
                
                // Movement
                
                case UpgradeNames.EngineHandling:
                    float EngineHandlingFn(int lvl) => (float) (1 + (.8f * lvl - 0.02f * lvl * lvl) * .5);
                    return this.GetUpgradeDataForEngineMultiplier(level, EngineHandlingFn, UpgradeNames.EngineHandling);
                case UpgradeNames.EngineStabilizationSpeed:
                    float EngineStabFn(int lvl) => (float) (1 + (.8f * lvl - 0.02f * lvl * lvl) * .5);
                    return this.GetUpgradeDataForEngineMultiplier(level, EngineStabFn, UpgradeNames.EngineStabilizationSpeed);
                case UpgradeNames.EngineAcceleration:
                    float EngineAccelerationFn(int lvl) => (float) (1 + (.8f * lvl - 0.02f * lvl * lvl) * .5);
                    return this.GetUpgradeDataForEngineMultiplier(level, EngineAccelerationFn, UpgradeNames.EngineAcceleration);

 
                default: throw new NotImplementedException();
            }
        }

        private UpgradeData GetUpgradeDataForEngineMultiplier(int level, Func<int, float> multiplierFn, UpgradeNames upgrade)
        {
            ValidateAndThrowOnUpgradeOutOfBounds(upgrade, level);
            var multiplierNow = multiplierFn(level - 1);
            var multiplierThen = multiplierFn(level);
            
            var deltaPercent = (multiplierThen - multiplierNow) * 100;
            var isNegative = deltaPercent < 0;
            if (isNegative)
            {
                deltaPercent = -deltaPercent;
            }

            var prefix = isNegative ? "-" : "+";
            var message = $"{prefix} {Math.Round(deltaPercent, 2)}%";

            return new UpgradeData(level, GetCostForWeaponMultipliers(level), multiplierNow, multiplierThen, message);
        }

        private UpgradeData GetUpgradeDataForWeaponMultiplier(int level, Func<int, float> multiplierFn, UpgradeNames upgrade)
        {
            ValidateAndThrowOnUpgradeOutOfBounds(upgrade, level);
            

            var multiplierNow = multiplierFn(level - 1);
            var multiplierThen = multiplierFn(level);
            
            var deltaPercent = (multiplierThen - multiplierNow) * 100;
            var isNegative = deltaPercent < 0;
            if (isNegative)
            {
                deltaPercent = -deltaPercent;
            }

            var prefix = isNegative ? "-" : "+";
            var message = $"{prefix} {Math.Round(deltaPercent, 2)}%";

            return new UpgradeData(level, GetCostForWeaponMultipliers(level), multiplierNow, multiplierThen, message);
        }

        private UpgradeData GetUpgradeDataForRocketChargeSpeed(int level)
        {
            ValidateAndThrowOnUpgradeOutOfBounds(UpgradeNames.RocketChargeSpeed, level);

            float SecondsRocketRechargeFn(int lvl) => (float)(6 - 2 * Math.Log(lvl + 1));

            var timeNow = SecondsRocketRechargeFn(level - 1);
            var timeThen = SecondsRocketRechargeFn(level);
            var message = $"- {Math.Round(timeNow - timeThen, 3)}s";

            return new UpgradeData(level, GetCostForLevelRocketRechargeSpeed(level), timeNow, timeThen, message);

        }

        

        private UpgradeData GetUpgradeDataForMaxRockets(int level)
        {
            ValidateAndThrowOnUpgradeOutOfBounds(UpgradeNames.MaxRockets, level);


            uint MaxRocketsFn(int lvl) => (uint)(3 + lvl);

            var maxRocketsNow = MaxRocketsFn(level - 1);
            var maxRocketsThen = MaxRocketsFn(level);

            return new UpgradeData(level, GetCostForLevelMaxRockets(level), maxRocketsNow, maxRocketsThen,
                $"+ {maxRocketsThen - maxRocketsNow} Rockets");

        }
        

        private UpgradeData GetUpgradeDataForLevelHealth(int level)
        {
            ValidateAndThrowOnUpgradeOutOfBounds(UpgradeNames.Health, level);


            uint HealthFn(int lvl) => 300 + (uint) (100 * lvl - lvl * lvl);

            var healthNow = HealthFn(level - 1);
            var healthAfterUpgrade = HealthFn(level);

            var label = $"+ {healthAfterUpgrade - healthNow} HP";

            return new UpgradeData(level, GetCostForLevelHealth(level), healthNow, healthAfterUpgrade, label);
        }

        
        
        #region Cost Functions

        private static uint GetCostForLevelHealth(int level) => (uint)Math.Ceiling(Math.Pow(level, 1.4f));
        private static uint GetCostForLevelMaxRockets(int level) => (uint) Math.Ceiling(Math.Pow(3 * level, 1.4f));
        private static uint GetCostForLevelRocketRechargeSpeed(int level) => (uint) Math.Ceiling(Math.Pow(3 * level, 1.4f));

        private static uint GetCostForWeaponMultipliers(int level) => (uint)Math.Ceiling(Math.Pow(level, 1.4f));
        
        #endregion
        

    }
}
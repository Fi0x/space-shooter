using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem.CostAndGain
{
    public abstract class UpgradeSystemCostAndGainLookupScriptableObject : ScriptableObject
    {
        protected abstract Dictionary<UpgradeNames, uint> MaxLevel { get; }

        protected virtual void ValidateInput(UpgradeNames upgrade, int level)
        {
            if (!this.MaxLevel.ContainsKey(upgrade))
            {
                throw new NotSupportedException("Upgrade " + upgrade + " is not supported.");
            }

            if (level < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level),
                    "Level needs to be a positive integer (including 0)");
            }
        }

        public UpgradeData GetUpgradeDataForLevel(UpgradeNames upgrade, int level)
        {
            Debug.Log("Getting upgrade data for level: " + upgrade);
            this.ValidateInput(upgrade, level);
            return this.GetUpgradeDataForLevelImpl(upgrade, level);
        }

        protected abstract UpgradeData GetUpgradeDataForLevelImpl(UpgradeNames upgrade, int level);

        public bool CanUpgrade(UpgradeNames upgrade, int level)
        {
            Debug.Log("Checking if can be upgraded: " + upgrade);
            this.ValidateInput(upgrade, level);
            return this.CanUpgradeImpl(upgrade, level);
        }

        protected abstract bool CanUpgradeImpl(UpgradeNames upgrade, int level);
    }
}
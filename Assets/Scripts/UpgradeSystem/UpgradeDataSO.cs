using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.Rendering;
using UpgradeSystem.CostAndGain;

namespace UpgradeSystem
{
    [CreateAssetMenu(fileName = "new UpgradeData", menuName = "Upgrades/Data")]
    public class UpgradeDataSO : ScriptableObject
    {
        public int freePoints = 0;

        [SerializeField]
        private SerializedDictionary<UpgradeNames, Upgrade> upgrades = new SerializedDictionary<UpgradeNames, Upgrade>();

        [SerializeField] private UpgradeSystemCostAndGainLookupScriptableObject upgradeLookupSO;

        private void OnEnable()
        {
            if (this.upgradeLookupSO is null)
            {
                throw new NullReferenceException("Upgrade Lookup is not set.");
            }
        }


        public IReadOnlyDictionary<UpgradeNames, Upgrade> Upgrades => this.upgrades;

        public float GetValue(UpgradeNames type)
        {
            if (this.upgrades.ContainsKey(type))
            {
                var currentLevel = this.upgrades[type].points;
                return this.upgradeLookupSO.GetUpgradeDataForLevel(type, currentLevel).ToValue;
            }

            return 0;
        }

        public UpgradeData GetNextUpgrade(UpgradeNames type)
        {
            return this.upgradeLookupSO.GetUpgradeDataForLevel(type, upgrades[type].points + 1);
        }

        public int GetPoints(UpgradeNames type)
        {
            if (this.upgrades.ContainsKey(type))
            {
                return this.upgrades[type].points;
            }
            
            return 0;
        }

        public Upgrade GetUpgrade(UpgradeNames type)
        {
            if (!this.upgrades.ContainsKey(type))
            {
                throw new Exception("Upgrade Type is not supported");
            }
            
            return this.upgrades[type];
        }

        public void AddPoints(UpgradeNames type, int points)
        {
            var u = GetUpgrade(type);
            if (u == null)
            {
                //upgrades.Add(new Upgrade(type, CalculationType.Linear, points));
                Debug.Log("Upgrade was missing");
                return;
            }
            u.points += points;
        }

        [ContextMenu("Reset Data")]
        public void ResetData()
        {
            freePoints = 0;
            foreach (var entry in this.upgrades.Values)
            {
                entry.points = 1;
            }
        }

        [ContextMenu("Hard Reset")]
        public void HardReset()
        {
            freePoints = 0;
            upgrades.Clear();
            bool isCopy = false;
            foreach (var type in (UpgradeNames[])Enum.GetValues(typeof(UpgradeNames)))
            { 
                upgrades[type] = (new Upgrade(type, 1));
            }
        }

        public IList<UpgradeNames> GetAllUpgradeable()
        {
            var valuesThatCanBeUpgraded = from val in this.upgrades.Keys
                where this.upgradeLookupSO.CanUpgrade(val, this.upgrades[val].points)
                select val;
            return valuesThatCanBeUpgraded.ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    [CreateAssetMenu(fileName = "new UpgradeData", menuName = "Upgrades/Data")]
    public class UpgradeDataSO : ScriptableObject
    {
        public int freePoints = 0;
        public List<Upgrade> upgrades = new List<Upgrade>();

        public float GetValue(UpgradeNames type)
        {
            foreach (var upgrade in upgrades)
            {
                if (upgrade.type == type)
                    return upgrade.GetValue();
            }

            return 0;
        }

        public int GetPoints(UpgradeNames type)
        {
            foreach (var upgrade in upgrades)
            {
                if (upgrade.type == type)
                    return upgrade.points;
            }

            return 0;
        }

        public Upgrade GetUpgrade(UpgradeNames type)
        {
            return upgrades.Find(upgrade => upgrade.type == type);
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
            foreach (var upgrade in upgrades)
            {
                upgrade.points = 1;
            }
        }

        [ContextMenu("Hard Reset")]
        public void HardReset()
        {
            freePoints = 0;
            upgrades.Clear();
            bool isCopy = false;
            foreach (var type in Enum.GetNames(typeof(UpgradeNames)))
            {
                if(type != "Unknown")
                    upgrades.Add(new Upgrade(Upgrade.GetTypeFromDisplayName(type), CalculationType.Linear, 1));
            }
        }
    }
}
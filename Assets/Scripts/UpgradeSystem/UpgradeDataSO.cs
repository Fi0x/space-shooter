using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    [CreateAssetMenu(fileName = "new UpgradeData", menuName = "Upgrades/Data")]
    public class UpgradeDataSO : ScriptableObject
    {
        public int freePoints = 0;
        public List<Upgrade> upgrades;

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

        public void AddPoints(UpgradeNames type, int points)
        {
            var u = upgrades.Find(upgrade => upgrade.type == type);
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
    }
}
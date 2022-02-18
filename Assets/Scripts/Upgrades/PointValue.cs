using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    public class PointValue : MonoBehaviour
    {
        private Text text;
        private void Start()
        {
            this.text = this.gameObject.GetComponent<Text>();
            this.UpdateValue();

            UpgradeMenuValues.UpgradeCompletedEvent += (sender, args) => { this.UpdateValue(); };
        }

        private void UpdateValue()
        {
            this.text.text = UpgradeStats.FreeUpgradePoints.ToString();
        }
    }
}
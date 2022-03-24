using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UpgradeSystem;

namespace UI.Upgrade
{
    public class UpgradeField : MonoBehaviour
    {
        [Header("Dependencies")] public UpgradeScreen upgradeScreen;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private TextMeshProUGUI downgradeCostText;

        [Header("Values")] public UpgradeNames type = UpgradeNames.Unknown;

        public void UpdateField()
        {
            nameText.text = type.ToString();
            pointsText.text = upgradeScreen.upgradeData.GetPoints(type).ToString();
            upgradeCostText.text = "-" + upgradeScreen.CalculateUpgradeCost(type) + " Points";
            downgradeCostText.text = "+" + upgradeScreen.CalculateUpgradeCost(upgradeScreen.upgradeData.GetPoints(type) - 1) + " Points";
            increaseButton.interactable = upgradeScreen.upgradeData.freePoints > upgradeScreen.CalculateUpgradeCost(type);
            decreaseButton.interactable = upgradeScreen.upgradeData.GetPoints(type) > 1;
        }

        public void BuyIncrease()
        {
            upgradeScreen.PurchaseUpgrade(type, true);
        }

        public void BuyDecrease()
        {
            upgradeScreen.PurchaseUpgrade(type, false);
        }
    }
}
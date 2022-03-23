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

        [Header("Values")] public UpgradeNames type = UpgradeNames.Unknown;

        public void UpdateField()
        {
            nameText.text = type.ToString();
            pointsText.text = upgradeScreen.upgradeData.GetPoints(type).ToString();
            increaseButton.interactable = upgradeScreen.upgradeData.freePoints > 0;
            decreaseButton.interactable = upgradeScreen.upgradeData.GetPoints(type) > 1;
        }

        public void BuyIncrease()
        {
            upgradeScreen.PurchaseUpgrade(type, 1);
        }

        public void BuyDecrease()
        {
            upgradeScreen.PurchaseUpgrade(type, -1);
        }
    }
}
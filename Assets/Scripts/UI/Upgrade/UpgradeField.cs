using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UpgradeSystem;

namespace UI.Upgrade
{
    public class UpgradeField : MonoBehaviour
    {
        [Header("Dependencies")] public UpgradeScreen upgradeScreen;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private TextMeshProUGUI fromText;
        [SerializeField] private TextMeshProUGUI toText;
        [SerializeField] private TextMeshProUGUI gainText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private TextMeshProUGUI buyButtonText;
        [SerializeField] private Button increaseButton;

        [Header("Values")]
        public UpgradeNames type;
        private int startPoints;

        private void Start()
        {
        }

        public void UpdateField()
        {
            var upgradeObject = upgradeScreen.upgradeData.GetNextUpgrade(type);
            
            image.sprite = upgradeScreen.spriteLookup.GetSprite(type);
            nameText.text = type.ToString();

            var canUpgrade = upgradeScreen.upgradeData.GetAllUpgradeable().Contains(type);
            
   
            pointsText.text = upgradeScreen.upgradeData.GetPoints(type).ToString();
            fromText.text = upgradeObject.FromValue.ToString("F2");
            toText.text = canUpgrade ? upgradeObject.ToValue.ToString("F2") : "";
            gainText.text = canUpgrade ? upgradeObject.UpgradeString : "";
            descriptionText.text = UpgradeDescriptionHelper.GetDescriptionForUpgrade(type);
            upgradeCostText.text = canUpgrade ? upgradeScreen.CalculateUpgradeCost(type) + " Points" : "";
            increaseButton.interactable = upgradeScreen.upgradeData.freePoints >= upgradeScreen.CalculateUpgradeCost(type) && canUpgrade;
            buyButtonText.text = canUpgrade ? "Upgrade" : "Max Reached";
        }

        public void BuyIncrease()
        {
            upgradeScreen.PurchaseUpgrade(type, true);
        }
    }
}
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
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private TextMeshProUGUI fromText;
        [SerializeField] private TextMeshProUGUI toText;
        [SerializeField] private TextMeshProUGUI gainText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private Button increaseButton;

        [Header("Values")]
        public UpgradeNames type = UpgradeNames.Health;

        private void Start()
        {
        }

        public void UpdateField()
        {
            image.sprite = upgradeScreen.spriteLookup.GetSprite(type);
            nameText.text = type.ToString();
            //TODO
            pointsText.text = upgradeScreen.upgradeData.GetPoints(type).ToString();
            fromText.text = "from points";
            toText.text = "to points";
            gainText.text = "gain text";
            descriptionText.text = "a description";
            upgradeCostText.text = "-" + upgradeScreen.CalculateUpgradeCost(type) + " Points";
            increaseButton.interactable = upgradeScreen.upgradeData.freePoints >= upgradeScreen.CalculateUpgradeCost(type);
        }

        public void BuyIncrease()
        {
            upgradeScreen.PurchaseUpgrade(type, true);
        }
    }
}
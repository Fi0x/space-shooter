using System;
using Stats;
using UnityEngine;
using UnityEngine.UI;
using UpgradeSystem;

namespace UI.Upgrade
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private bool isIncrease;

        public static event EventHandler<UpgradePurchasedEventArgs> UpgradePurchasedEvent;

        public Enum Type;
        private Button button;

        private void Start()
        {
            this.button = this.GetComponent<Button>();
            this.UpdateVisibility();
            UpgradePurchasedEvent += (sender, args) => this.UpdateVisibility();
            UpgradeScreen.UpgradeScreenShownEvent += (sender, args) => { this.UpdateVisibility(); };
        }

        public void ButtonPressed()
        {
            var valueChange = this.isIncrease ? 1 : -1;
            UpgradeHandler.PurchaseUpgrade(this.Type, valueChange);
            UpgradePurchasedEvent?.Invoke(null, new UpgradePurchasedEventArgs(this.Type, valueChange));
            
            StatCollector.UpdateStat("Upgrades Purchased", valueChange);
        }

        private void UpdateVisibility()
        {
            if(this.isIncrease)
                this.button.interactable = UpgradeHandler.FreeUpgradePoints > 0;
            else
                this.button.interactable = UpgradeHandler.GetSpecificUpgrade(this.Type) > 1;
        }
        
        public class UpgradePurchasedEventArgs : EventArgs
        {
            public readonly Enum Type;
            public readonly int ValueChange;
        
            public UpgradePurchasedEventArgs(Enum upgradeType, int valueChange)
            {
                this.Type = upgradeType;
                this.ValueChange = valueChange;
            }
        }
    }
}
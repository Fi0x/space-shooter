using System;
using UnityEngine;
using Upgrades;

namespace UI.Upgrade
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private bool isIncrease;

        public static event EventHandler<UpgradePurchasedEventArgs> UpgradePurchasedEvent;

        public Enum Type;

        private void Start()
        {
            this.UpdateVisibility();
            UpgradePurchasedEvent += (sender, args) => this.UpdateVisibility();
        }

        public void ButtonPressed()
        {
            var valueChange = this.isIncrease ? 1 : -1;
            UpgradeHandler.PurchaseUpgrade(this.Type, valueChange);
            UpgradePurchasedEvent?.Invoke(null, new UpgradePurchasedEventArgs(this.Type, valueChange));
        }

        private void UpdateVisibility()
        {
            if(this.isIncrease)
                this.gameObject.SetActive(UpgradeHandler.FreeUpgradePoints > 0);
            else
                this.gameObject.SetActive(UpgradeHandler.GetSpecificUpgrade(this.Type) > 1);
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
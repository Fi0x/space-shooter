using System;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrades
{
    public class LevelTransitionMenu : MonoBehaviour
    {
        [SerializeField] private Scrollbar scrollbar;

        private static GameObject _upgradeMenu;
        private static Scrollbar _scrollbar;

        public static event EventHandler<UpgradePurchasedEventArgs> UpgradePurchasedEvent;

        private void Start()
        {
            _upgradeMenu = this.gameObject;
            _upgradeMenu.SetActive(false);

            _scrollbar = this.scrollbar;
        }

        public static void ShowUpgradeScreen()
        {
            GameManager.IsGamePaused = true;
            _upgradeMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _scrollbar.value = 1;
        }

        public static void LoadNextLevel()
        {
            GameManager.IsGamePaused = false;
            _upgradeMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.Instance.LoadNextLevel();
        }

        public static void UpgradeButtonPressed(int upgradeType)
        {
            var increase = false;
            if (upgradeType >= 100)
            {
                increase = true;
                upgradeType -= 100;
            }

            Upgrade type;
            var price = 1;
            
            switch (upgradeType)
            {
                case 0:
                    type = Upgrade.WeaponDamage;
                    price = 1;
                    break;
                case 1:
                    type = Upgrade.WeaponFireRate;
                    price = 1;
                    break;
                case 2:
                    type = Upgrade.WeaponProjectileSpeed;
                    price = 1;
                    break;
                case 3:
                    type = Upgrade.EngineAcceleration;
                    price = 1;
                    break;
                case 4:
                    type = Upgrade.EngineDeceleration;
                    price = 1;
                    break;
                case 5:
                    type = Upgrade.EngineLateralThrust;
                    price = 1;
                    break;
                case 61:
                    type = Upgrade.EngingRotationSpeedPitch;
                    price = 1;
                    break;
                case 62:
                    type = Upgrade.EngingRotationSpeedRoll;
                    price = 1;
                    break;
                case 63:
                    type = Upgrade.EngingRotationSpeedYaw;
                    price = 1;
                    break;
                case 7:
                    type = Upgrade.EngineStabilizationSpeed;
                    price = 1;
                    break;
                case 8:
                    type = Upgrade.Armor;
                    price = 1;
                    break;
                default:
                    return;
            }
            
            UpgradePurchasedEvent?.Invoke(null, new UpgradePurchasedEventArgs(type, increase, price));
        }

        public class UpgradePurchasedEventArgs : EventArgs
        {
            public readonly Upgrade Type;
            public readonly bool Increased;
            public readonly int Price;
        
            public UpgradePurchasedEventArgs(Upgrade upgradeType, bool increased, int price)
            {
                this.Type = upgradeType;
                this.Increased = increased;
                this.Price = price;
            }
        }
        
        public enum Upgrade
        {
            WeaponDamage,
            WeaponFireRate,
            WeaponProjectileSpeed,
            EngineAcceleration,
            EngineDeceleration,
            EngineLateralThrust,
            EngingRotationSpeedPitch,
            EngingRotationSpeedRoll,
            EngingRotationSpeedYaw,
            EngineStabilizationSpeed,
            Armor,
            Unknown
        }
    }
}
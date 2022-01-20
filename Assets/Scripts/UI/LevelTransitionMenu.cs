using System;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
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
            var type = UpgradePurchasedEventArgs.Upgrade.Armor;
            switch (upgradeType)
            {
                case 0:
                    type = UpgradePurchasedEventArgs.Upgrade.WeaponDamage;
                    break;
                case 1:
                    type = UpgradePurchasedEventArgs.Upgrade.WeaponFireRate;
                    break;
                case 2:
                    type = UpgradePurchasedEventArgs.Upgrade.WeaponProjectileSpeed;
                    break;
                case 3:
                    type = UpgradePurchasedEventArgs.Upgrade.EngineAcceleration;
                    break;
                case 4:
                    type = UpgradePurchasedEventArgs.Upgrade.EngineDeceleration;
                    break;
                case 5:
                    type = UpgradePurchasedEventArgs.Upgrade.EngineLateralThrust;
                    break;
                case 6:
                    type = UpgradePurchasedEventArgs.Upgrade.EngingRotationSpeed;
                    break;
                case 7:
                    type = UpgradePurchasedEventArgs.Upgrade.EngineStabilizationSpeed;
                    break;
                case 8:
                    type = UpgradePurchasedEventArgs.Upgrade.Armor;
                    break;
            }
            
            UpgradePurchasedEvent?.Invoke(null, new UpgradePurchasedEventArgs(type, increase));
        }
    }

    public class UpgradePurchasedEventArgs : EventArgs
    {
        public readonly Upgrade Type;
        public readonly bool Increased;
        
        public UpgradePurchasedEventArgs(Upgrade upgradeType, bool increased)
        {
            this.Type = upgradeType;
            this.Increased = increased;
        }
        
        public enum Upgrade
        {
            WeaponDamage,
            WeaponFireRate,
            WeaponProjectileSpeed,
            EngineAcceleration,
            EngineDeceleration,
            EngineLateralThrust,
            EngingRotationSpeed,
            EngineStabilizationSpeed,
            Armor
        }
    }
}
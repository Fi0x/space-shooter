using System;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class OverlayMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseObject;
        [SerializeField] private GameObject settingsObject;
        [SerializeField] private GameObject keyBindObject;
        [SerializeField] private GameObject statObject;
        [SerializeField] private TextMeshProUGUI menuTitle;
        [SerializeField] private Toggle speedIndicatorToggle;
        [SerializeField] private Toggle spaceDustToggle;
        [SerializeField] private Toggle movementModeToggle;

        private static GameObject _overlayMenu;
        private static GameObject _pauseMenu;
        private static GameObject _settingsMenu;
        private static GameObject _keyBindMenu;
        private static GameObject _statMenu;
        private static TextMeshProUGUI _menuTitle;
        private static Toggle _speedIndicatorToggle;
        private static Toggle _spaceDustToggle;
        private static Toggle _movementModeToggle;

        private void Start()
        {
            _overlayMenu = this.gameObject;
            _pauseMenu = this.pauseObject;
            _settingsMenu = this.settingsObject;
            _keyBindMenu = this.keyBindObject;
            _statMenu = this.statObject;
            _menuTitle = this.menuTitle;
            _speedIndicatorToggle = this.speedIndicatorToggle;
            _spaceDustToggle = this.spaceDustToggle;
            _movementModeToggle = this.movementModeToggle;
            
            _speedIndicatorToggle.isOn = true;
            _spaceDustToggle.isOn = false;
            _movementModeToggle.isOn = false;
            
            if(GameManager.IsGamePaused) Pause();
            else Resume();
            
            //SpeedIndicatorVisibilityChanged?.Invoke(null, new BoolEventChangerArgs { NewBoolValue = _speedIndicatorToggle.isOn });
            //SpaceDustVisibilityChanged?.Invoke(null, new BoolEventChangerArgs { NewBoolValue = _spaceDustToggle.isOn });
        }

        public static void Pause()
        {
            GameManager.IsGamePaused = true;
            _overlayMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _menuTitle.text = "Menu";
            _pauseMenu.SetActive(true);
            _settingsMenu.SetActive(false);
            _keyBindMenu.SetActive(false);
            _statMenu.SetActive(false);
        }

        public static void Resume()
        {
            GameManager.IsGamePaused = false;
            _overlayMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void Quit()
        {
            Resume();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.ReturnToMenu();
        }

        public static void Settings()
        {
            _menuTitle.text = "Settings";
            _pauseMenu.SetActive(false);
            _settingsMenu.SetActive(true);
            _keyBindMenu.SetActive(false);
            _statMenu.SetActive(false);
        }

        public static void KeyBinds()
        {
            _menuTitle.text = "Key Binds";
            _pauseMenu.SetActive(false);
            _settingsMenu.SetActive(false);
            _keyBindMenu.SetActive(true);
            _statMenu.SetActive(false);
        }

        public static void Stats()
        {
            _menuTitle.text = "Stats";
            _pauseMenu.SetActive(false);
            _settingsMenu.SetActive(false);
            _keyBindMenu.SetActive(false);
            _statMenu.SetActive(true);
        }

        public static void BackToMainOverlay()
        {
            _menuTitle.text = "Menu";
            _pauseMenu.SetActive(true);
            _settingsMenu.SetActive(false);
            _settingsMenu.SetActive(false);
            _keyBindMenu.SetActive(false);
            _statMenu.SetActive(false);
        }

        public static void InvokeSpeedIndicatorVisibilityChange()
        {
            SettingsManager.Instance.DisplaySpeedIndicator = !_speedIndicatorToggle.isOn;
        }
    }

    public class BoolEventChangerArgs : EventArgs
    {
        public bool NewBoolValue;
    }
}
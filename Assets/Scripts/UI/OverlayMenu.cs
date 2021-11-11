using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OverlayMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseObject;
        [SerializeField] private GameObject settingsObject;
        [SerializeField] private Text menuTitle;
        
        private static GameObject _overlayMenu;
        private static GameObject _pauseMenu;
        private static GameObject _settingsMenu;
        private static Text _menuTitle;

        private void Start()
        {
            _overlayMenu = this.gameObject;
            _pauseMenu = this.pauseObject;
            _settingsMenu = this.settingsObject;
            _menuTitle = this.menuTitle;
            
            if(GameManager.IsGamePaused) Pause();
            else Resume();
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
            Application.Quit();
        }

        public static void Settings()
        {
            _menuTitle.text = "Settings";
            _pauseMenu.SetActive(false);
            _settingsMenu.SetActive(true);
        }

        public static void BackToMainOverlay()
        {
            _menuTitle.text = "Menu";
            _pauseMenu.SetActive(true);
            _settingsMenu.SetActive(false);
        }
    }
}
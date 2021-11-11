using Manager;
using UnityEngine;

namespace UI
{
    public class OverlayMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseObject;
        [SerializeField] private GameObject settingsObject;
        
        private static GameObject _overlayMenu;
        private static GameObject _pauseMenu;
        private static GameObject _settingsMenu;

        private void Start()
        {
            _overlayMenu = this.gameObject;
            _pauseMenu = this.pauseObject;
            _settingsMenu = this.settingsObject;
            
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
            _pauseMenu.SetActive(false);
            _settingsMenu.SetActive(true);
        }

        public static void BackToMainOverlay()
        {
            _pauseMenu.SetActive(true);
            _settingsMenu.SetActive(false);
        }
    }
}
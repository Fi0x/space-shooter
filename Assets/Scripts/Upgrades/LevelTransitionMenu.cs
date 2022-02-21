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
    }
}
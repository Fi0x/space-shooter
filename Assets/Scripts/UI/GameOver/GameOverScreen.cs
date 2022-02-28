using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameOver
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private Scrollbar scrollbar;
        
        private static GameObject _gameOverMenu;
        private static Scrollbar _scrollbar;
        private void Start()
        {
            _gameOverMenu = this.gameObject;
            _gameOverMenu.SetActive(false);
            
            _scrollbar = this.scrollbar;
        }

        public static void ShowGameOverScreen()
        {
            GameManager.IsGamePaused = true;
            _gameOverMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _scrollbar.value = 1;
        }

        public static void ReturnToMenu()
        {
            GameManager.ResetGame();
            GameManager.Instance.LoadNextLevel();
            _gameOverMenu.SetActive(false);
            OverlayMenu.Pause();
        }
    }
}
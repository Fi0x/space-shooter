using Manager;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private static GameObject _pauseMenu;

    private void Start()
    {
        _pauseMenu = gameObject;
        _pauseMenu.SetActive(GameManager.IsGamePaused);
        Time.timeScale = GameManager.IsGamePaused ? 0f : 1;
    }

    public static void Pause()
    {
        GameManager.IsGamePaused = true;
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void Resume()
    {
        GameManager.IsGamePaused = false;
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void Quit()
    {
        Application.Quit();
    }

    public static void CustomTestAction()
    {
        GameManager.Instance.LoadNextLevel();
    }
}
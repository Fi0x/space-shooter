using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;
    private static GameObject pauseMenu;

    private void Start()
    {
        pauseMenu = gameObject;
        pauseMenu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0f : 1;
    }

    public static void Pause()
    {
        IsPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public static void Resume()
    {
        IsPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public static void Quit()
    {
        Application.Quit();
    }

    public static void CustomTestAction()
    {
        Debug.Log("Test button clicked");
    }
}
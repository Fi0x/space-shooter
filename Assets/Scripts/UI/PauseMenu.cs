using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [HideInInspector] public static bool IsPaused;
    [SerializeField] private GameObject pauseMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            IsPaused = !IsPaused;
            pauseMenu.SetActive(IsPaused);
            Time.timeScale = IsPaused ? 0f : 1;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] public static bool IsPaused = true;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private void Start()
    {
        if (IsPaused)
        {
            pauseMenu.SetActive(IsPaused);
            Time.timeScale = IsPaused ? 0f : 1;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            IsPaused = !IsPaused;
            pauseMenu.SetActive(IsPaused);
            Time.timeScale = IsPaused ? 0f : 1;
        }
    }
}
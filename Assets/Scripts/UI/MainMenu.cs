using System;
using UnityEngine;
using System.Collections;
using Manager;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public BootScript bootScript;
        public GameObject settingsPrefab;
        public CanvasGroup fadeCanvasGroup;
        public Image loadingBar;
        public float fadeTime = 5f;
        public float delay = 2f;

        private void Start()
        {
            StartCoroutine(FadeIn(fadeTime));
        }

        public void StartGame()
        {
            GameManager.Instance.LoadNextLevel();
        }
        
        public void OpenSettings()
        {
            Debug.Log("Open settings here!");
        }
    
        public void QuitGame()
        {
            //save?
            Application.Quit();
        }

        private IEnumerator FadeIn(float time)
        {
            fadeCanvasGroup.alpha = 1f;
            
            float elapsed1 = 0f;
            while (elapsed1 < delay)
            {
                elapsed1 += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0f, 1f, elapsed1 / time);
                yield return null;
            }
            
            float elapsed = 0f;
            while (elapsed < time)
            {
                elapsed += Time.unscaledDeltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / time);
                yield return null;
            }

            fadeCanvasGroup.alpha = 0f;
        }
    }
}

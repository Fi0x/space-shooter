using System;
using UnityEngine;
using System.Collections;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public BootScript bootScript;
        public GameObject settingsPrefab;
        public CanvasGroup fadeCanvasGroup;
        public float fadeTime = 5f;
        public float delay = 2f;

        private void Start()
        {
            StartCoroutine(FadeIn(fadeTime));
        }

        public void StartGame()
        {
            bootScript.StartLoading();
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
            yield return new WaitForSeconds(delay);
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

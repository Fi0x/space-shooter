using Enemy.Station;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private Image barImage;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float updateSpeedSeconds = 0.2f;

        [SerializeField] private StationController controller;
        
        public void SetHealth(StationController controller)
        {
            this.controller = controller;
            controller.OnHealthPctChanged += HandleHealthChange;
        }

        public void SetVisible(bool isVisible)
        {
            StartCoroutine(isVisible ? FadeIn() : FadeOut());
        }

        private void HandleHealthChange(float pct)
        {
            StartCoroutine(ChangeToPct(pct));
        }

        private IEnumerator ChangeToPct(float pct)
        {
            float preChangePct = barImage.fillAmount;
            float elapsed = 0f;

            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                barImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
                yield return null;
            }

            barImage.fillAmount = pct;
        }
        
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;

            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / updateSpeedSeconds);
                yield return null;
            }

            canvasGroup.alpha = 1;
        }
        
        private IEnumerator FadeOut()
        {
            float elapsed = 0f;

            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / updateSpeedSeconds);
                yield return null;
            }

            canvasGroup.alpha = 0;
        }
    }
}
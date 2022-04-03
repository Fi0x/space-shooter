using System.Collections;
using Components;
using UnityEngine;

namespace UI
{
    public class ShieldVFX : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public AnimationCurve curve;
        public float flashDuration = 0.2f;

        private void OnEnable()
        {
            PlayerHealth.OnDamageTaken += FlashOnce;
        }

        private void OnDisable()
        {
            PlayerHealth.OnDamageTaken -= FlashOnce;
        }

        private void FlashOnce(PlayerHealth health)
        {
            StartCoroutine(FadeIn());
        }

        public IEnumerator FadeIn()
        {
            float currentTime = 0f;
            float duration = flashDuration;

            while (currentTime < duration)
            {
                float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
                canvasGroup.alpha = curve.Evaluate(alpha);
                currentTime += Time.deltaTime;

                yield return null;
            }

            canvasGroup.alpha = 0f;
        }
    }
}

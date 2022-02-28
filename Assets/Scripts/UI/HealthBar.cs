using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image barImage;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float updateSpeedSeconds = 0.2f;
        [SerializeField] private float offset = 2f;
        [SerializeField] private Health health;
        [SerializeField] private Vector2 minMaxSize = new Vector2(30f, 60f);
        [SerializeField] private Vector2 rangeValues = new Vector2(50f, 100f);
        
        public bool visibleWhenFull = false;
        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
            if(visibleWhenFull) return;
            canvasGroup.alpha = 0f;
        }

        public void SetHealth(Health health)
        {
            this.health = health;
            health.OnHealthPctChanged += HandleHealthChange;
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

        private void LateUpdate()
        {
            if (cam != null)
            {
                var position = health.transform.position;
                transform.position = cam.WorldToScreenPoint(position + Vector3.up * offset);
                
                float f = Vector3.Distance(position, cam.transform.position);
                f = Mathf.Clamp(f, rangeValues.x, rangeValues.y);
                f = f / Mathf.Abs(rangeValues.y - rangeValues.x);
                
                float size = Mathf.Lerp(minMaxSize.y, minMaxSize.x, f);
                GetComponent<RectTransform>().sizeDelta = new Vector2((int)size, (int)(size / 5));
                
                
                var camTransform = cam.transform;
                float dot = Vector3.Dot(camTransform.forward,
                    (position - camTransform.position).normalized);
                //dot = 1f;
                
                if (dot >= 0)
                {
                    if (barImage.fillAmount >= 1f)
                    {
                        canvasGroup.alpha = 0f;
                        return;
                    }

                    canvasGroup.alpha = 1f;
                }
            }
        }

        private void OnDestroy()
        {
            //health.OnHealthPctChanged -= HandleHealthChange;
        }
    }
}

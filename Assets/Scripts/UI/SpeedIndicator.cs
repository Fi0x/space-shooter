using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpeedIndicator : MonoBehaviour
    {
        [Header("SpeedIndicator")]
        [SerializeField] private Slider slider;
        [SerializeField] private RectTransform fill;
        [SerializeField] private GameObject spaceDust;

        private float offset = 30;

        private void Start()
        {
            this.slider.value = this.offset;
            this.gameObject.SetActive(SettingsManager.IsSpeedIndicatorVisible);
            this.spaceDust.SetActive(SettingsManager.IsSpaceDustVisible);

            SettingsManager.SpeedIndicatorVisibilityChanged += (sender, args) => { this.gameObject.SetActive(SettingsManager.IsSpeedIndicatorVisible); };
            SettingsManager.SpaceDustVisibilityChanged += (sender, args) => { this.spaceDust.SetActive(SettingsManager.IsSpaceDustVisible); };
        }

        public void SetMaxSpeed(float maxSpeed)
        {
            this.offset = maxSpeed;
            this.slider.maxValue = maxSpeed;
        }

        public void SetCurrentSpeed(float speed)
        {
            var calculatedValue = speed;
            
            if (speed >= 0)
            {
                this.fill.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                this.fill.localRotation = Quaternion.Euler(180, 0, 0);
                calculatedValue *= -1;
            }
            
            if (calculatedValue < 0) calculatedValue = 0;
            if (calculatedValue > this.slider.maxValue) calculatedValue = this.slider.maxValue;
            this.slider.value = calculatedValue;
        }
    }
}
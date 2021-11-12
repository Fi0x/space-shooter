using Ship;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpeedIndicator : MonoBehaviour
    {

        [Header("SpeedIndicator")]
        [SerializeField] private Slider slider;

        private float offset = 30;

        private void Start()
        {
            this.slider.value = this.offset;
        }

        public void SetMaxSpeed(float maxSpeed)
        {
            this.offset = maxSpeed;
            this.slider.maxValue = 2 * maxSpeed;
        }

        public void SetCurrentSpeed(float speed)
        {
            var calculatedValue = speed + this.offset;
            if (calculatedValue < 0) calculatedValue = 0;
            if (calculatedValue > this.slider.maxValue) calculatedValue = this.slider.maxValue;
            this.slider.value = calculatedValue;
        }
    }
}
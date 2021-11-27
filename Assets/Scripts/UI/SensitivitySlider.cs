using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SensitivitySlider : MonoBehaviour
    {
        [SerializeField] private InputField valueField;
        private Slider slider;

        private void Start()
        {
            this.slider = this.gameObject.GetComponent<Slider>();
            
            this.valueField.text = $"{InputManager.MouseSensitivity}";
            this.slider.value = InputManager.MouseSensitivity;
        }

        public void SliderUpdated()
        {
            var newValue = this.slider.value;

            this.valueField.text = $"{newValue}";
            InputManager.MouseSensitivity = newValue;
        }

        public void ValueUpdated()
        {
            var newValue = float.Parse(this.valueField.text);
            if (newValue > 1)
            {
                newValue = 1;
                this.valueField.text = "1";
            }

            if (newValue <= 0)
            {
                newValue = 0.001f;
                this.valueField.text = "0.001";
            }

            this.slider.value = newValue;
            InputManager.MouseSensitivity = newValue;
        }
    }
}

using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private InputField valueField;
        [SerializeField] private Slider slider;

        private void Start()
        {
            this.valueField.text = $"{InputManager.MouseSensitivity}";
            this.slider.value = InputManager.MouseSensitivity;
        }

        public void SliderUpdated()
        {
            var newValue = this.slider.value;

            this.valueField.text = $"{newValue}";
            AudioManager.instance.UpdateGeneralVolume(newValue);
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
            AudioManager.instance.UpdateGeneralVolume(newValue);
        }
    }
}

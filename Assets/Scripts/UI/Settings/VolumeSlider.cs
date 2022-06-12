using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private TMP_InputField valueField;
        [SerializeField] private SliderType soundCategory = SliderType.Music;
        
        private Slider slider;

        private const float MinValue = -80;
        private const float MaxValue = 20;

        private void Start()
        {
            this.slider = this.gameObject.GetComponent<Slider>();

            this.valueField.text = this.GetVolumeForType().ToString();
            this.slider.value = this.GetVolumeForType();
        }

        public void SliderUpdated()
        {
            if(this.slider == null)
                this.slider = this.gameObject.GetComponent<Slider>();

            var newValue = this.slider.value;

            this.valueField.text = $"{newValue}";
            this.UpdateVolume(newValue);
        }

        public void ValueUpdated()
        {
            var newValue = float.Parse(this.valueField.text);
            if (newValue > MaxValue)
            {
                newValue = MaxValue;
                this.valueField.text = $"{MaxValue}";
            }

            if (newValue < MinValue)
            {
                newValue = MinValue;
                this.valueField.text = $"{MinValue}";
            }

            this.slider.value = newValue;
            this.UpdateVolume(newValue);
        }

        private void UpdateVolume(float newValue)
        {
            switch (this.soundCategory)
            {
                case SliderType.Music:
                    SettingsManager.Instance.MusicVolume = newValue;
                    break;
                case SliderType.Effects:
                    SettingsManager.Instance.EffectsVolume = newValue;
                    break;
                case SliderType.Master:
                    SettingsManager.Instance.MasterVolume = newValue;
                    break;
            }
        }

        private float GetVolumeForType()
        {
            switch (this.soundCategory)
            {
                case SliderType.Music:
                    return SettingsManager.Instance.MusicVolume;
                case SliderType.Effects:
                    return SettingsManager.Instance.EffectsVolume;
                case SliderType.Master:
                    return SettingsManager.Instance.MasterVolume;
            }

            return 0;
        }

        private enum SliderType
        {
            Music,
            Effects,
            Master
        }
    }
}

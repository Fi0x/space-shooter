using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private InputField valueField;
        [SerializeField] private SliderType soundCategory = SliderType.Music;
        
        private Slider slider;

        private const float MinValue = -80;
        private const float MaxValue = 20;

        private void Start()
        {
            this.slider = this.gameObject.GetComponent<Slider>();

            this.valueField.text = "0";
            this.slider.value = 0;
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
                    AudioManager.instance.UpdateMusicAmbientVolume(newValue);
                    break;
                case SliderType.Effects:
                    AudioManager.instance.UpdateEffectsVolume(newValue);
                    break;
                case SliderType.Master:
                    AudioManager.instance.UpdateMasterVolume(newValue);
                    break;
            }
        }

        private enum SliderType
        {
            Music,
            Effects,
            Master
        }
    }
}

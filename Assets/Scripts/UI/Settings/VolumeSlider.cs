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

        private void Start()
        {
            this.slider = this.gameObject.GetComponent<Slider>();

            var value = this.soundCategory == SliderType.Music ? 0.3f : AudioManager.EffectsVolume;
            
            this.valueField.text = $"{value}";
            this.slider.value = value;
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
                    AudioManager.EffectsVolume = newValue;
                    break;
            }
        }

        private enum SliderType
        {
            Music,
            Effects
        }
    }
}

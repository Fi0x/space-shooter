using System;
using UnityEngine;

namespace Manager
{
    public class SettingsManager
    {
        private static SettingsManager _instance;
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingsManager();

                return _instance;
            }
        }

        private float mouseSensitivity;//TODO Use this new system for mouse-sensitivity (after new-input-system merge)
        public float MouseSensitivity
        {
            get => this.mouseSensitivity;
            set
            {
                SettingsChangedEvent?.Invoke(null, null);
                this.mouseSensitivity = value;
                this.WriteSettingsToDisk();
            }
        }
        
        private float masterVolume;
        public float MasterVolume
        {
            get => this.masterVolume;
            set
            {
                SettingsChangedEvent?.Invoke(null, null);
                this.masterVolume = value;
                this.WriteSettingsToDisk();
            }
        }
        
        private float musicVolume;
        public float MusicVolume
        {
            get => this.musicVolume;
            set
            {
                SettingsChangedEvent?.Invoke(null, null);
                this.musicVolume = value;
                this.WriteSettingsToDisk();
            }
        }
        
        private float effectsVolume;
        public float EffectsVolume
        {
            get => this.effectsVolume;
            set
            {
                SettingsChangedEvent?.Invoke(null, null);
                this.effectsVolume = value;
                this.WriteSettingsToDisk();
            }
        }

        public static event EventHandler SettingsChangedEvent;
        
        private SettingsManager()
        {
            this.LoadSettingsFromDisk();
        }

        private void LoadSettingsFromDisk()
        {
            this.mouseSensitivity = PlayerPrefs.GetFloat(this.mouseSensitivity.ToString(), 0.3f);
            this.masterVolume = PlayerPrefs.GetFloat(this.masterVolume.ToString(), 0);
            this.musicVolume = PlayerPrefs.GetFloat(this.musicVolume.ToString(), 0);
            this.effectsVolume = PlayerPrefs.GetFloat(this.effectsVolume.ToString(), 0);
        }
        private void WriteSettingsToDisk()
        {
            PlayerPrefs.SetFloat(this.mouseSensitivity.ToString(), this.mouseSensitivity);
            PlayerPrefs.SetFloat(this.masterVolume.ToString(), this.masterVolume);
            PlayerPrefs.SetFloat(this.musicVolume.ToString(), this.musicVolume);
            PlayerPrefs.SetFloat(this.effectsVolume.ToString(), this.effectsVolume);
        }
    }
}
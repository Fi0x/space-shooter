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

        private bool displaySpeedIndicator;
        public bool DisplaySpeedIndicator
        {
            get => this.displaySpeedIndicator;
            set
            {
                SettingsChangedEvent?.Invoke(null, null);
                this.displaySpeedIndicator = value;
                this.WriteSettingsToDisk();
            }
        }
        
        private bool displaySpaceDust;
        public bool DisplaySpaceDust
        {
            get => this.displaySpaceDust;
            set
            {
                SettingsChangedEvent?.Invoke(null, null);
                this.displaySpaceDust = value;
                this.WriteSettingsToDisk();
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
            this.DisplaySpeedIndicator = PlayerPrefs.GetInt(this.DisplaySpeedIndicator.ToString(), 1) == 1;
            this.DisplaySpaceDust = PlayerPrefs.GetInt(this.DisplaySpaceDust.ToString(), 1) == 1;
            this.MouseSensitivity = PlayerPrefs.GetFloat(this.MouseSensitivity.ToString(), 0.3f);
            this.MasterVolume = PlayerPrefs.GetFloat(this.MasterVolume.ToString(), 0);
            this.MusicVolume = PlayerPrefs.GetFloat(this.MusicVolume.ToString(), 0);
            this.EffectsVolume = PlayerPrefs.GetFloat(this.EffectsVolume.ToString(), 0);
        }
        private void WriteSettingsToDisk()
        {
            PlayerPrefs.SetInt(this.DisplaySpeedIndicator.ToString(), this.DisplaySpeedIndicator ? 1 : 0);
            PlayerPrefs.SetInt(this.DisplaySpaceDust.ToString(), this.DisplaySpaceDust ? 1 : 0);
            PlayerPrefs.SetFloat(this.MouseSensitivity.ToString(), this.MouseSensitivity);
            PlayerPrefs.SetFloat(this.MasterVolume.ToString(), this.MasterVolume);
            PlayerPrefs.SetFloat(this.MusicVolume.ToString(), this.MusicVolume);
            PlayerPrefs.SetFloat(this.EffectsVolume.ToString(), this.EffectsVolume);
        }
    }
}
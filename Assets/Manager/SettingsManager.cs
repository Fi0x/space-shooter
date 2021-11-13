using System;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private Toggle speedIndicatorToggle;
        [SerializeField] private Toggle spaceDustToggle;

        public static float MouseSensitivity = 0.25f;

        private static Toggle _speedIndicatorToggle;
        private static Toggle _spaceDustToggle;

        public static bool IsSpeedIndicatorVisible => _speedIndicatorToggle.isOn;
        public static bool IsSpaceDustVisible => _spaceDustToggle.isOn;

        public static event EventHandler SpeedIndicatorVisibilityChanged;
        public static event EventHandler SpaceDustVisibilityChanged;

        private void Start()
        {
            _speedIndicatorToggle = this.speedIndicatorToggle;
            _spaceDustToggle = this.spaceDustToggle;
            
            _speedIndicatorToggle.isOn = true;
            _spaceDustToggle.isOn = true;
        }

        public static void InvokeSpeedIndicatorVisibilityChange()
        {
            SpeedIndicatorVisibilityChanged?.Invoke(null, null);
        }
        public static void InvokeSpaceDustVisibilityChange()
        {
            SpaceDustVisibilityChanged?.Invoke(null, null);
        }
    }
}
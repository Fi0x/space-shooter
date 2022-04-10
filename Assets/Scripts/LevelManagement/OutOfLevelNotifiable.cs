using System;
using UnityEngine;
using UnityEngine.Events;
using UpgradeSystem;

namespace LevelManagement
{
    public class OutOfLevelNotifiable : MonoBehaviour
    {
        [SerializeField] private OutOfLevelNotifierScriptableObject config;
        [SerializeField] private float pollingRateSeconds = 0.2f;

        [SerializeField, ReadOnlyInspector]
        private OutOfLevelNotifierScriptableObject.OutOfLevelState currentState =
            OutOfLevelNotifierScriptableObject.OutOfLevelState.NoNoise;

        [SerializeField] public UnityEvent<OutOfLevelNotifierScriptableObject.OutOfLevelState> newStateEvent;
        [SerializeField] public UnityEvent<float> newNoiseLevelEvent;

        private float currentTimer = 0;

        private void Update()
        {
            currentTimer += Time.deltaTime;
            if (currentTimer > pollingRateSeconds)
            {
                currentTimer %= pollingRateSeconds;
                this.InvokeUpdate();
            }
        }

        private void InvokeUpdate()
        {
            var newState = this.config.GetCurrentState(this);
            if (currentState != newState)
            {
                this.currentState = newState;
            }

            if (currentState == OutOfLevelNotifierScriptableObject.OutOfLevelState.LowNoise ||
                currentState == OutOfLevelNotifierScriptableObject.OutOfLevelState.NoiseAndWarning ||
                currentState == OutOfLevelNotifierScriptableObject.OutOfLevelState.ConnectionLost)
            {
                this.UpdateNoisePercent(this.config.GetCurrentNoiseFraction(this));
            }
        }
        

        private void UpdateNoisePercent(float currentNoiseFraction)
        {
            this.newNoiseLevelEvent.Invoke(currentNoiseFraction);
        }
        
    }
}
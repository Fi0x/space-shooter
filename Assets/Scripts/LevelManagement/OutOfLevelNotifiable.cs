using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Manager;
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

        [Header("UI Elements")] 
        [SerializeField] private GameObject lowSignalUI;
        [SerializeField] private GameObject criticalSignalUI;
        [SerializeField] private GameObject noSignalUI;
        

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

            if (currentState == OutOfLevelNotifierScriptableObject.OutOfLevelState.ConnectionLost)
            {
                StartCoroutine(PlayerLostCoroutine());
            }

            this.UpdateUi(newState);
        }

        private void UpdateUi(OutOfLevelNotifierScriptableObject.OutOfLevelState newState)
        {
            this.HideAllUi();

            var uiToActivate = newState switch
            {
                OutOfLevelNotifierScriptableObject.OutOfLevelState.NoNoise => null,
                OutOfLevelNotifierScriptableObject.OutOfLevelState.LowNoise => this.lowSignalUI,
                OutOfLevelNotifierScriptableObject.OutOfLevelState.NoiseAndWarning => this.criticalSignalUI,
                OutOfLevelNotifierScriptableObject.OutOfLevelState.ConnectionLost => this.noSignalUI,
                _ => null
            };
            if (uiToActivate != null)
            {
                uiToActivate.SetActive(true);
            }
        }

        private void HideAllUi()
        {
            // First unset all
            foreach (var entry in new[] {this.criticalSignalUI, this.lowSignalUI, this.noSignalUI})
            {
                entry.SetActive(false);
            }        
        }

        private IEnumerator PlayerLostCoroutine()
        {
            yield return new WaitForSeconds(1f);
            // Reset UI
            this.HideAllUi();
            GameManager.Instance.GameOver();
        }


        private void UpdateNoisePercent(float currentNoiseFraction)
        {
            this.newNoiseLevelEvent.Invoke(currentNoiseFraction);
        }
        
    }
}
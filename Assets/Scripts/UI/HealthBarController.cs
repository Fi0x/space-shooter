using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine.SceneManagement;

namespace UI
{
    public class HealthBarController : MonoBehaviour
    {
        public HealthBar healthBarPrefab;

        private Dictionary<Health, HealthBar> healthBars = new Dictionary<Health, HealthBar>();

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelLoaded;
        }

        private void Awake()
        {
            healthBars = new Dictionary<Health, HealthBar>();
            Health.OnHealthAdded += AddHealthBar;
            Health.OnHealthRemoved += RemoveHealthBar;
        }

        private void AddHealthBar(Health health)
        {
            if (!healthBars.ContainsKey(health))
            {
                var healthBar = Instantiate(healthBarPrefab, transform);
                healthBars.Add(health, healthBar);
                healthBar.SetHealth(health);
            }
        }
        
        private void RemoveHealthBar(Health health)
        {
            if (healthBars.ContainsKey(health))
            {
                Destroy(healthBars[health].gameObject);
                healthBars.Remove(health);
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelLoaded;
            Health.OnHealthAdded -= AddHealthBar;
            Health.OnHealthRemoved -= RemoveHealthBar;
            foreach (var health in healthBars.Keys)
            {
                Destroy(healthBars[health].gameObject);
            }
            healthBars.Clear();
        }

        private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var healthBar in healthBars)
            {
                if (healthBar.Key == null) healthBars.Remove(healthBar.Key);
            }
        }
    }
}
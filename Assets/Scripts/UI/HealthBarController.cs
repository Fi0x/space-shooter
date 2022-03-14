using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Components;
using Enemy.Station;
using UnityEngine.SceneManagement;

namespace UI
{
    public class HealthBarController : MonoBehaviour
    {
        public HealthBar healthBarPrefab;
        public StationController bossController;
        public BossHealthBar bossHealthBar;

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
            StationController.OnBossHealthAdded += SetBossHealthBar;
            StationController.OnBossHealthRemoved += RemoveBossHealthBar;
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
        
        private void SetBossHealthBar(StationController controller)
        {
            bossController = controller;
            bossHealthBar.SetHealth(controller);
            bossHealthBar.SetVisible(true);
        }
        
        private void RemoveBossHealthBar(StationController controller)
        {
            bossController = null;
            bossHealthBar.SetVisible(false);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelLoaded;
            Health.OnHealthAdded -= AddHealthBar;
            Health.OnHealthRemoved -= RemoveHealthBar;
            StationController.OnBossHealthAdded -= SetBossHealthBar;
            StationController.OnBossHealthRemoved -= RemoveBossHealthBar;
            foreach (var health in healthBars.Keys)
            {
                Destroy(healthBars[health].gameObject);
            }
            healthBars.Clear();
            bossController = null;
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
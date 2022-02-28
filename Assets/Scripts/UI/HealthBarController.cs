using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Components;

namespace UI
{
    public class HealthBarController : MonoBehaviour
    {
        public HealthBar healthBarPrefab;

        private Dictionary<Health, HealthBar> healthBars = new Dictionary<Health, HealthBar>();

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
            Health.OnHealthAdded -= AddHealthBar;
            Health.OnHealthRemoved -= RemoveHealthBar;
            foreach (var health in healthBars.Keys)
            {
                Destroy(healthBars[health].gameObject);
            }
            healthBars.Clear();
        }
    }
}
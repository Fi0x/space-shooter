using System;
using System.Collections.Generic;
using Components;
using HealthSystem;
using UnityEngine;

namespace Enemy.Station
{
    public class StationController : MonoBehaviour
    {
        [Header("Station Health")]
        public List<Health> healthTargets;
        public int currentHealth;
        public int maxHealth;

        [Header("Parts & Turrets")]
        [SerializeField] public List<StationPart> parts;
        [SerializeField] public List<Turret> turrets;
        
        public static event Action<StationController> OnBossHealthAdded = delegate { };
        public static event Action<StationController> OnBossHealthRemoved = delegate { };
        public event Action<float> OnHealthPctChanged;

        private void Start()
        {
            OnBossHealthAdded?.Invoke(this);
            OnHealthPctChanged?.Invoke(1f);
        }

        public void Reset()
        {
            currentHealth = 0;
            maxHealth = 0;
        }

        private void Update()
        {
            if (currentHealth <= 0)
            {
                DestroyStation();
            }
        }

        private void DestroyStation()
        {
            OnBossHealthRemoved?.Invoke(this);
        }

        public void AddTarget(Health health)
        {
            healthTargets.Add(health);
            currentHealth++;
            maxHealth++;
        }
        
        private void Awake()
        {
            Health.OnHealthRemoved += OnTargetDestroyed;
        }

        private void OnTargetDestroyed(Health obj)
        {
            if(!healthTargets.Contains(obj)) return;
            healthTargets.Remove(obj);
            currentHealth--;
            OnHealthPctChanged?.Invoke((float)currentHealth/maxHealth);
        }

        private void OnDisable()
        {
            DestroyStation();
        }
    }
}
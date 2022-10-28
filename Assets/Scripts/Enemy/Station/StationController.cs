using System;
using System.Collections.Generic;
using Components;
using HealthSystem;
using Manager;
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

        public bool isInit = false;
        public bool isDying = false;
        
        public static event Action<StationController> OnBossHealthAdded = delegate { };
        public static event Action<StationController> OnBossHealthRemoved = delegate { };
        public event Action<float> OnHealthPctChanged;
        

        public void InvokeStartEvents()
        {
            Debug.Log("invoked station events");
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
            Debug.Log("updating!");
            if(!isInit) return;
            if(isDying) return;
            if (currentHealth <= 0 || healthTargets.Count <= 0)
            {
                Debug.Log("Station destroyed!");
                DestroyStation();
            }
        }

        private void DestroyStation()
        {
            isDying = true;
            OnBossHealthRemoved?.Invoke(this);
            GameManager.Instance.playerUpgrades.freePoints += 10;
            foreach (var part in parts)
            {
                part.stationRemover.Explode();
            }
            
            GameManager.Instance.CompleteLevel();
        }

        public void AddTarget(Health health)
        {
            healthTargets.Add(health);
            currentHealth++;
            maxHealth++;
        }
        
        private void Start()
        {
            Health.OnHealthRemoved += this.OnTargetDestroyed;
            GameManager.Instance.CreateNewText("Destroy the station or at least 80% of enemy ships", 0, "mainGoal");
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
            //DestroyStation();
        }
    }
}
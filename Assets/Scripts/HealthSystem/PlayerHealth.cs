using System;
using Enemy;
using Manager;
using Stats;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UpgradeSystem;

namespace Components
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        
        [SerializeField] private UpgradeDataSO upgradeData;
        [SerializeField] private int baseHealth = 1000;
        
        private int maxHealth;
        public int MaxHealth
        {
            get
            {
                if(upgradeData != null) 
                    return this.maxHealth + (int)(upgradeData.GetValue(UpgradeNames.Health) * 100);
                return this.maxHealth;
            }
            set
            {
                this.maxHealth = value;
                currentHealth = maxHealth;
                HealthBar.SetMaxHealth(this.MaxHealth);
            }
        }
        
        private float currentHealth;
        public float CurrentHealth
        {
            get => this.currentHealth;
            set
            {
                this.currentHealth = value;
                if (this.currentHealth > this.MaxHealth) this.currentHealth = this.MaxHealth;
                this.HealthBar.SetCurrentHealth((int)Math.Round(this.currentHealth));
                
            }
        }
        
        private FixedHealthBar healthBar;
        private FixedHealthBar HealthBar
        {
            get
            {
                if(!this.healthBar) this.healthBar = this.GetComponentInChildren<FixedHealthBar>();
                return this.healthBar;
            }
        }
        
        public UnityEvent onDamageTaken;
        public UnityEvent onDeath;
        
        public static event Action<PlayerHealth> OnDamageTaken;
        public static event Action<PlayerHealth> OnDeath; 
        
        private void Start()
        {
            MaxHealth = baseHealth;
            CurrentHealth = this.MaxHealth;
        }
        
        public void TakeDamage(float damage)
        {
            onDamageTaken?.Invoke();
            OnDamageTaken?.Invoke(this);
            
            this.CurrentHealth -= damage;
            StatCollector.UpdateGeneralStat("Damage Taken", damage);
            
            if(this.CurrentHealth > 0)
                return;

            onDeath?.Invoke();
            OnDeath?.Invoke(this);
            GameManager.Instance.GameOver();
        }
    }
}
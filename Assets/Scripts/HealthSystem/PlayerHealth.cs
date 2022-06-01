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
        [SerializeField] private AudioClip hitSound;

        [ReadOnlyInspector] [SerializeField] private int maxHealth = 0;
        
        
        public int MaxHealth
        {
            get => (int)Math.Round(upgradeData.GetValue(UpgradeNames.Health));
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
                this.HealthBar.SetCurrentHealth((int)Math.Ceiling(this.currentHealth));
                
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
            MaxHealth = (int)this.upgradeData.GetValue(UpgradeNames.Health);
            CurrentHealth = this.MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            onDamageTaken?.Invoke();
            OnDamageTaken?.Invoke(this);

            var audioSrc = this.GetComponent<AudioSource>();
            audioSrc.clip = this.hitSound;
            audioSrc.Play();

            this.CurrentHealth -= damage;
            StatCollector.UpdateGeneralStat("Damage Taken", damage);

            if (this.CurrentHealth > 0)
                return;

            onDeath?.Invoke();
            OnDeath?.Invoke(this);
            GameManager.Instance.GameOver();
        }
    }
}
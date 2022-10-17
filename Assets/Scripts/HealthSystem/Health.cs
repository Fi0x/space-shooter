using System;
using Components;
using Enemy;
using Manager;
using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace HealthSystem
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int baseHealth = 1000;

        private int maxHealth;
        public int MaxHealth
        {
            get => this.maxHealth;
            set
            {
                this.maxHealth = value;
                currentHealth = maxHealth;
            }
        }
        
        public static event Action<Health> OnHealthAdded = delegate { };
        public static event Action<Health> OnHealthRemoved = delegate { };
        public event Action<float> OnHealthPctChanged;

        [Header("Death VFX")]
        public GameObject deathVFX;
        public float vfxLifetime = 4.5f;

        private float currentHealth;
        public float CurrentHealth
        {
            get => this.currentHealth;
            set
            {
                this.currentHealth = value;
                if (this.currentHealth > this.MaxHealth) this.currentHealth = this.MaxHealth;
                float currentHealthPct = (float)currentHealth / maxHealth;
                OnHealthPctChanged?.Invoke(currentHealthPct);
            }
        }
        
        public UnityEvent onDamageTaken;
        public UnityEvent onDeath;

        private void Start()
        {
            OnHealthAdded(this);
            
            this.MaxHealth = baseHealth;
            this.CurrentHealth = this.MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            onDamageTaken?.Invoke();
            this.CurrentHealth -= damage;
            
            if(this.CurrentHealth > 0)
                return;
            
            onDeath?.Invoke();
            
            //spawn vfx
            if (deathVFX != null)
            {
                GameObject vfx = Instantiate(deathVFX, transform.position, transform.rotation);
                Destroy(vfx, vfxLifetime);
            }

            AudioManager.instance.Play("EnemyKilled");
            
            Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            OnHealthRemoved(this);
        }
    }
}
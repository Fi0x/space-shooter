using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Manager;
using UI.Upgrade;
using UnityEngine;
using Upgrades;

namespace Components
{
    public class Health : MonoBehaviour, IUpgradeable
    {
        [SerializeField] private bool isPlayer;

        private readonly Dictionary<Enum, int> upgrades = new Dictionary<Enum, int>();
        private int maxHealth;
        public int MaxHealth
        {
            get
            {
                if(this.upgrades.ContainsKey(UI.Upgrade.Upgrades.UpgradeNames.Health)) 
                    return this.maxHealth + this.upgrades[UI.Upgrade.Upgrades.UpgradeNames.Health] * 10;
                return this.maxHealth;
            }
            set
            {
                this.maxHealth = value;
                this.HealthBar.SetMaxHealth(this.MaxHealth);
            }
        }
        
        

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
                this.HealthBar.SetCurrentHealth(this.currentHealth);
            }
        }

        private HealthBar healthBar;
        private HealthBar HealthBar
        {
            get
            {
                if(!this.healthBar) this.healthBar = this.GetComponentInChildren<HealthBar>();
                return this.healthBar;
            }
        }

        private void Start()
        {
            if(this.isPlayer)
                this.ResetUpgrades();
            
            this.MaxHealth = 1000;
            this.CurrentHealth = this.MaxHealth;

            if (!this.isPlayer)
                return;
            
            UpgradeButton.UpgradePurchasedEvent += (sender, args) =>
            {
                if (this.upgrades.ContainsKey(args.Type))
                    this.upgrades[args.Type] = args.ValueChange;
            };
        }

        public void TakeDamage(float damage)
        {
            this.CurrentHealth -= damage;

            if (this.isPlayer)
            {
                StatCollector.FloatStats[StatCollector.StatValues.DamageTaken] += damage;

                // shieldVFX
                if(TryGetComponent(out ShieldVFX shieldVFX))
                {
                    StartCoroutine(shieldVFX.FadeIn());
                }
            }

            if(this.CurrentHealth > 0)
                return;
            
            //spawn vfx
            if (deathVFX != null)
            {
                GameObject vfx = Instantiate(deathVFX, transform.position, transform.rotation);
                Destroy(vfx, vfxLifetime);
            }
            
            if(this.TryGetComponent(out Boid boid))
                boid.RemoveBoidFromAssignedFlock();
            
            if(this.isPlayer)
                GameManager.GameOver();
            else
            {
                StatCollector.IntStats[StatCollector.StatValues.EnemiesKilled]++;
                UpgradeStats.FreeUpgradePoints++;
                Destroy(this.gameObject);
            }
        }

        public void ResetUpgrades()
        {
            this.upgrades.Clear();
            
            this.upgrades.Add(UI.Upgrade.Upgrades.UpgradeNames.Health, 1);
            
            UpgradeHandler.RegisterUpgrades(this, this.upgrades.Keys.ToList());
        }
    }
}
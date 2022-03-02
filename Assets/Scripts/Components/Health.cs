using System;
using Enemy;
using Manager;
using UI;
using UnityEngine;
using Upgrades;
using System.Collections;

namespace Components
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private bool isPlayer;
        private int maxHealth;
        public int MaxHealth
        {
            get => this.maxHealth + UpgradeStats.ArmorLevel * 10;
            set
            {
                this.maxHealth = value;
                if(!generateHealthBar)
                    HealthBar.SetMaxHealth(this.MaxHealth);
            }
        }
        
        public static event Action<Health> OnHealthAdded = delegate { };
        public static event Action<Health> OnHealthRemoved = delegate { };
        public event Action<float> OnHealthPctChanged;
        public bool generateHealthBar = false;

        public AnimationCurve flashingCurve;
        public float flashingDuration;
        public Renderer renderer;

        public GameObject deathVFX;
        public float vfxLifetime = 4.5f;

        private int currentHealth;
        public int CurrentHealth
        {
            get => this.currentHealth;
            set
            {
                this.currentHealth = value;
                if (this.currentHealth > this.MaxHealth) this.currentHealth = this.MaxHealth;
                if (generateHealthBar)
                {
                    float currentHealthPct = (float)currentHealth / maxHealth;
                    OnHealthPctChanged?.Invoke(currentHealthPct);
                }
                else
                {
                    this.HealthBar.SetCurrentHealth(this.currentHealth);
                }
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

        private void Start()
        {
            if (generateHealthBar) OnHealthAdded(this);
            this.MaxHealth = 1000;
            this.CurrentHealth = this.MaxHealth;

            UpgradeButton.UpgradePurchasedEvent += (sender, args) =>
            {
                if (args.Type == UpgradeButton.Upgrade.Armor)
                {
                    UpgradeStats.ArmorLevel += args.Increased ? 1 : -1;
                    UpgradeMenuValues.InvokeUpgradeCompletedEvent(args);
                }
            };
        }

        public void TakeDamage(int damage)
        {
            this.CurrentHealth -= damage;
            //flashing
            StopCoroutine(Flash(0f));
            StartCoroutine(Flash(flashingDuration));
            
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
                StatCollector.EnemiesKilled++;
                UpgradeStats.FreeUpgradePoints++;
                if (generateHealthBar) OnHealthRemoved(this);
                Destroy(this.gameObject);
            }
        }

        IEnumerator Flash(float time)
        {
            for (float t = 0f; t < time; t += Time.deltaTime)
            {
                renderer.material.SetFloat("_FlashingStrength", flashingCurve.Evaluate(t / time));
                yield return null;
            }
        }
    }
}
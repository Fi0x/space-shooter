using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Manager;
using UI;
using UnityEngine;
using System.Collections;
using Stats;
using UpgradeSystem;

namespace Components
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private bool isPlayer;
        [SerializeField] private UpgradeDataSO upgradeData;

        private int maxHealth;
        public int MaxHealth
        {
            get
            {
                 if(upgradeData != null) 
                     return this.maxHealth + (int)(upgradeData.GetValue(UpgradeNames.Health) * 10);
                 return this.maxHealth;
            }
            set
            {
                this.maxHealth = value;
                currentHealth = maxHealth;
                if(!generateHealthBar)
                    HealthBar.SetMaxHealth(this.MaxHealth);
            }
        }
        
        public static event Action<Health> OnHealthAdded = delegate { };
        public static event Action<Health> OnHealthRemoved = delegate { };
        public event Action<float> OnHealthPctChanged;
        public bool generateHealthBar = false;

        [Header("Feedback")]
        public AnimationCurve flashingCurve;
        public float flashingDuration;
        public List<Renderer> renderers;

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
                if (generateHealthBar)
                {
                    float currentHealthPct = (float)currentHealth / maxHealth;
                    OnHealthPctChanged?.Invoke(currentHealthPct);
                }
                else
                {
                    this.HealthBar.SetCurrentHealth((int)Math.Round(this.currentHealth));
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
        }

        public void TakeDamage(float damage)
        {
            this.CurrentHealth -= damage;
            //flashing
            StopCoroutine(Flash(0f));
            StartCoroutine(Flash(flashingDuration));
            

            if (this.isPlayer)
            {
                StatCollector.UpdateGeneralStat("Damage Taken", damage);

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
                GameManager.Instance.GameOver();
            else
            {
                StatCollector.UpdateGeneralStat("Enemies Killed", 1);
                GameManager.Instance.playerUpgrades.freePoints++;
                if (generateHealthBar) OnHealthRemoved(this);
                Destroy(this.gameObject);
            }
        }

        IEnumerator Flash(float time)
        {
            for (float t = 0f; t < time; t += Time.deltaTime)
            {
                foreach (var renderer in renderers)
                {
                    renderer.material.SetFloat("_FlashingStrength", flashingCurve.Evaluate(t / time));
                }
                yield return null;
            }
        }
    }
}
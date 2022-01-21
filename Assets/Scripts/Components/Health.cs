using Enemy;
using Manager;
using UnityEngine;

namespace Components
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private bool isPlayer;
        private int maxHealth;
        public int MaxHealth
        {
            get => this.maxHealth;
            set
            {
                this.maxHealth = value;
                this.HealthBar.SetMaxHealth(this.MaxHealth);
            }
        }

        public GameObject deathVFX;
        public float vfxLifetime = 4f;

        private int currentHealth;
        public int CurrentHealth
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
            this.MaxHealth = 1000;
            this.CurrentHealth = this.MaxHealth;
        }

        public void TakeDamage(int damage)
        {
            this.CurrentHealth -= damage;
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
                Destroy(this.gameObject);
        } 
    }
}
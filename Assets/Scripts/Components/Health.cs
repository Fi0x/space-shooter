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
            // if object is a Boid, remove it from the Flock
            if(TryGetComponent(out Boid boid))
            {
                boid.RemoveBoidFromAssignedFlock();
            }
            Destroy(this.gameObject);
        } 
    }
}
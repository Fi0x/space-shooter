using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    //
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;

    private HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        //
        this.maxHealth = 1000;
        this.currentHealth = this.maxHealth;

        //
        this.healthBar = this.GetComponentInChildren<HealthBar>();

        //
        this.healthBar.SetMaxHealth(this.maxHealth);
    }

    public void TakeDamage(int damage)
    {
        this.currentHealth -= damage;
        this.healthBar.SetCurrentHealth(this.currentHealth);

        if(this.currentHealth <= 0)
        {
            Destroy(this.gameObject);
        } 
    }
}

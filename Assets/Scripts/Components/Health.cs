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
        maxHealth = 1000;
        currentHealth = maxHealth;

        //
        healthBar = GetComponentInChildren<HealthBar>();

        //
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetCurrentHealth(currentHealth);

        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        } 
    }
}

using System.Collections.Generic;
using Components;
using UnityEngine;

namespace Enemy.Station
{
    public class StationController : MonoBehaviour, IDamageable
    {
        [Header("Station Values")]
        [SerializeField] public float currentHealth;
        [SerializeField] private float maxHealth;

        [Header("Parts & Turrets")]
        [SerializeField] public List<StationPart> parts;
        [SerializeField] public List<Turret> turrets;

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0f;
        }
    }
}
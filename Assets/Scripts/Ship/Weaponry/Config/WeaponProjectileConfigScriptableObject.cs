using System;
using Components;
using UnityEngine;

namespace Ship.Weaponry.Config
{
    [CreateAssetMenu(fileName = "WeaponConfigScriptableObject", menuName = "ScriptableObject/Ship/WeaponConfig/Projectile",
        order = 50)]
    public class WeaponProjectileConfigScriptableObject : WeaponConfigScriptableObject
    {
        [SerializeField] private float timeToLive;
        [SerializeField] private AnimationCurve damageOverTimeNormalized = null!;
        [SerializeField] private GameObject projectilePrefab = null!;
        [SerializeField] private GameObject muzzlePrefab = null!;
        [SerializeField] private float projectileSpeed;

        public AnimationCurve DamageOverTimeNormalized => this.damageOverTimeNormalized;
        public GameObject ProjectilePrefab => this.projectilePrefab;
        public GameObject MuzzlePrefab => muzzlePrefab;
        public float TimeToLive => this.timeToLive;

        public float ProjectileSpeed => this.projectileSpeed;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _ = this.damageOverTimeNormalized ?? throw new NullReferenceException();
            _ = (object)this.projectilePrefab ?? throw new NullReferenceException();
            if (!projectilePrefab.TryGetComponent<WeaponProjectile>(out _))
            {
                throw new Exception("The provided Prefab is not a Projectile");
            }

            if (projectileSpeed <= 0)
            {
                throw new Exception("Projectile Speed must be positive");
            }
        }
    }

}
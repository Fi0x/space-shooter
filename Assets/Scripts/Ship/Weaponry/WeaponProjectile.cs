using System;
using Components;
using Ship.Sensors;
using UnityEngine;
using UnityEngine.VFX;

namespace Ship.Weaponry
{
    public class WeaponProjectile : MonoBehaviour
    {
        public GameObject muzzlePrefab = null!;
        public GameObject impactPrefab = null!;

        private bool isInit;
        [SerializeField] private Rigidbody rb = null!;
        [SerializeField] private GameObject trail = null!;

        private AnimationCurve damageOverTimeNormalized = null!;
        private double startTime;
        private float timeToLive;

        public event Action<int, WeaponHitInformation> ProjectileHitSomethingEvent;

        public float DamageMultiplier { get; set; } = 1;
        
        private void Start()
        {
            var vfx = this.GetComponentInChildren<VisualEffect>();
            if (vfx != null)
            {
                vfx.Play();
            }

            if (this.muzzlePrefab != null)
            {
                var muzzle = Instantiate(this.muzzlePrefab, this.transform)!;
                Destroy(muzzle, 1f);
            }
            
            this.trail.SetActive(false);
            this.Invoke(nameof(this.MakeTrailVisible), 0.4f);
        }

        public void Initialize(Vector3 directionAndVelocity, AnimationCurve damageOverTimeNormalized, Quaternion rotation, float ttl)
        {
            if (damageOverTimeNormalized == null) throw new ArgumentNullException(nameof(damageOverTimeNormalized));
            if (this.isInit)
            {
                throw new Exception("Already initialized");
            }

            this.timeToLive = ttl;
            Destroy(this.gameObject, this.timeToLive);
            this.transform.LookAt(this.transform.position + directionAndVelocity);

            this.gameObject.transform.rotation = rotation;

            this.damageOverTimeNormalized = damageOverTimeNormalized;
            this.rb.velocity = directionAndVelocity;
            this.isInit = true;
            this.startTime = Time.timeAsDouble;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ShouldCollide(other))
            {
                var timeOnImpact = Time.timeAsDouble - this.startTime;

                if (this.impactPrefab != null)
                {
                    var pos = this.transform.position;
                    var closestPoint = other.ClosestPoint(pos);
                    var impact = Instantiate(this.impactPrefab, closestPoint, Quaternion.LookRotation(pos - closestPoint));
                    Destroy(impact, 2f);
                }

                
                var damageTaken = (int) (DamageMultiplier * this.damageOverTimeNormalized.Evaluate((float) timeOnImpact / this.timeToLive));
                var weaponHitInformation = new WeaponHitInformation(
                    WeaponHitInformation.WeaponType.Projectile, 
                    damageTaken, 
                    other.gameObject.GetComponent<SensorTarget>()
                );
                if (other.gameObject.TryGetComponent(out Health health))
                {
                    // The hit "thing" can take damage
                    health.TakeDamage(damageTaken);
                }
                this.ProjectileHitSomethingEvent?.Invoke(other.gameObject.layer, weaponHitInformation);

                Destroy(this.gameObject);
            }    
        }

        private static bool ShouldCollide(Component c)
        {
            return c.gameObject.layer == LayerMask.NameToLayer("Scenery") || c.gameObject.layer == LayerMask.NameToLayer("Enemy");
        }

        private void MakeTrailVisible()
        {
            this.trail.SetActive(true);
        }
    }
}
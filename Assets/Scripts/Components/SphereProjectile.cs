using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Components
{
    public class SphereProjectile : MonoBehaviour
    {
        public GameObject muzzlePrefab;
        public GameObject impactPrefab;
    
        private bool isInit;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float timeToLive;

        private AnimationCurve damageOverTime;
        private double startTime;

        public event Action<int> ProjectileHitSomethingEvent;


        private void Start()
        {
            Destroy(this.gameObject, this.timeToLive);
            var vfx = this.GetComponentInChildren<VisualEffect>();
            if (vfx != null)
            {
                vfx.Play();
            }

            if (this.muzzlePrefab != null)
            {
                var muzzle = Instantiate(this.muzzlePrefab, this.transform);
                Destroy(muzzle, 1f);
            }
        }

        public void InitializeDirection(Vector3 velocity, AnimationCurve damageOverTimeCurve, Quaternion rotation)
        {
            if (this.isInit)
            {
                throw new Exception("Already initialized");
            }
            this.transform.LookAt(this.transform.position + velocity);

            this.gameObject.transform.rotation = rotation;

            this.damageOverTime = damageOverTimeCurve;
            this.rb.velocity = velocity;
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

                if (other.gameObject.TryGetComponent(out Health health))
                {
                    health.TakeDamage((int)this.damageOverTime.Evaluate((float)timeOnImpact));
                }
                this.ProjectileHitSomethingEvent?.Invoke(other.gameObject.layer);

                Destroy(this.gameObject);
            }    
        }

        private static bool ShouldCollide(Component c)
        {
            return c.gameObject.layer == LayerMask.NameToLayer("Scenery") || c.gameObject.layer == LayerMask.NameToLayer("Enemy");
            //return (this.layerMask & c.gameObject.layer) > 0;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.VFX;

public class SphereProjectile : MonoBehaviour
{
    public GameObject muzzlePrefab;
    public GameObject impactPrefab;
    
    private bool isInit = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float timeToLive;

    [SerializeField, ReadOnlyInspector] private LayerMask layerMask;

    private AnimationCurve damageOverTime;
    private double startTime = 0f;

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

    public void InitializeDirection(Vector3 velocity, LayerMask layerMask, AnimationCurve damageOverTime, Quaternion rotation)
    {
        if (this.isInit)
        {
            throw new Exception("Already initialized");
        }
        this.transform.LookAt(this.transform.position + velocity);

        this.layerMask = layerMask;
        this.gameObject.transform.rotation = rotation;

        this.damageOverTime = damageOverTime;
        this.rb.velocity = velocity;
        this.isInit = true;
        this.startTime = Time.timeAsDouble;
    }



    private void OnTriggerEnter(Collider other)
    {

        if (this.ShouldCollide(other))
        {
            var timeOnImpact = Time.timeAsDouble - this.startTime;

            if (this.impactPrefab != null)
            {
                var closestPoint = other.ClosestPoint(this.transform.position);
                var impact = Instantiate(this.impactPrefab, closestPoint,
                    Quaternion.LookRotation(this.transform.position - closestPoint));
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

    private bool ShouldCollide(Component c)
    {
        return c.gameObject.layer == LayerMask.NameToLayer("Scenery") || c.gameObject.layer == LayerMask.NameToLayer("Enemy");
        //return (this.layerMask & c.gameObject.layer) > 0;
    }
}

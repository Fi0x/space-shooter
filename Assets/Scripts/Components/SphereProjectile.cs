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
    private string tagWhichCanTakeDamage;

    private AnimationCurve damageOverTime;
    private double startTime = 0f;


    private void Start()
    {
        Destroy(this.gameObject, timeToLive);
        var vfx = GetComponentInChildren<VisualEffect>();
        if (vfx != null)
        {
            vfx.Play();
        }

        if (muzzlePrefab != null)
        {
            var muzzle = Instantiate(muzzlePrefab, transform);
            Destroy(muzzle, 1f);
        }
    }

    public void InitializeDirection(Vector3 velocity, bool isFriendly, AnimationCurve damageOverTime)
    {
        if (this.isInit)
        {
            throw new Exception("Already initialized");
        }
        this.transform.LookAt(transform.position + velocity);

        this.damageOverTime = damageOverTime;
        this.tagWhichCanTakeDamage = isFriendly ? "Enemy" : "Player" ;
        this.rb.velocity = velocity;
        this.isInit = true;
        this.startTime = Time.timeAsDouble;
    }



    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.TryGetComponent(out Health health) && this.ShouldCollide(other))
        {
            var timeOnImpact = Time.timeAsDouble - this.startTime;
            
            health.TakeDamage((int)this.damageOverTime.Evaluate((float)timeOnImpact));
            if (impactPrefab != null)
            {
                var closestPoint = other.ClosestPoint(transform.position);
                var impact = Instantiate(impactPrefab, closestPoint, Quaternion.LookRotation(transform.position - closestPoint));
                Destroy(impact, 2f);
            }
            Destroy(this.gameObject);
        }    
    }

    private bool ShouldCollide(Component c)
    {
        return c.gameObject.CompareTag(this.tagWhichCanTakeDamage);
    }
}

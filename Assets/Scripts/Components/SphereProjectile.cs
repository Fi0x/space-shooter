using System;
using UnityEngine;
using UnityEngine.VFX;

public class SphereProjectile : MonoBehaviour
{
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
            Destroy(this.gameObject);
        }    
    }

    private bool ShouldCollide(Component c)
    {
        return c.gameObject.CompareTag(this.tagWhichCanTakeDamage);
    }
}

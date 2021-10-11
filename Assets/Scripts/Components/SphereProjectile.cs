using System;
using UnityEngine;

public class SphereProjectile : MonoBehaviour
{

    private int Damage = 100;
    private bool isInit = false;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float timeToLive;
    private string tagWhichCanTakeDamage;

    private void Start()
    {
        Destroy(this.gameObject, timeToLive);
    }

    public void InitializeDirection(Vector3 velocity, bool isFriendly)
    {
        if (this.isInit)
        {
            throw new Exception("Already initialized");
        }

        this.tagWhichCanTakeDamage = isFriendly ? "Enemy" : "Player" ;
        this.rb.velocity = velocity;
        this.isInit = true;
    }



    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.TryGetComponent(out Health health) && this.ShouldCollide(other))
        {
            health.TakeDamage(Damage);
            Destroy(this.gameObject);
        }    
    }

    private bool ShouldCollide(Component c)
    {
        return c.gameObject.CompareTag(this.tagWhichCanTakeDamage);
    }
}

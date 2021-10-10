using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereProjectile : MonoBehaviour
{

    private float speed = 20f;
    private int Damage = 100;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage(Damage);
            Destroy(this.gameObject);
        }    
    }
}

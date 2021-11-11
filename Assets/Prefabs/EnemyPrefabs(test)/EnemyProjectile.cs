using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage = 100;
    Rigidbody rigidBody;
    Vector3 direction;

    float speed = 25.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.direction = (GameManager.Instance.Player.transform.position - this.transform.position).normalized;
        this.rigidBody = this.GetComponent<Rigidbody>();

        Destroy(this.gameObject, 20.0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.rigidBody.velocity = this.direction * this.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Scenery"))
        {
            if(other.TryGetComponent(out Health health))
            {
                health.TakeDamage(this.damage);

            }
            Destroy(this.gameObject);
        }
    }
}

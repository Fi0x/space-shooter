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
        direction = (GameManager.Instance.Player.transform.position - this.transform.position).normalized;
        rigidBody = GetComponent<Rigidbody>();

        Destroy(gameObject, 20.0f);
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(other.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}

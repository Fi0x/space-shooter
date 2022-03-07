using Components;
using Manager;
using Ship;
using UnityEngine;

namespace Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        private const int Damage = 100;
        private Rigidbody rigidBody;
        private Vector3 direction;

        private void Start()
        {
            this.direction = (GameManager.Instance.Player.transform.position - this.transform.position).normalized;
            this.rigidBody = this.GetComponent<Rigidbody>();

            Destroy(this.gameObject, 20.0f);
        }

        private void Update()
        {
            // TODO
            //this.rigidBody.velocity = 1.1f * ShipMovementHandler.TotalMaxSpeed * this.direction;
            this.rigidBody.velocity = 1.1f * 50 * this.direction;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Scenery"))
            {
                if(other.TryGetComponent(out Health health))
                {
                    health.TakeDamage(Damage);

                }
                Destroy(this.gameObject);
            }
        }
    }
}
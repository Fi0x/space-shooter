using Components;
using Manager;
using Ship;
using UnityEngine;

namespace Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        public int Damage = 100;
        public float speed = 100f;
        public float timeToLive = 10f;
        private Rigidbody rigidBody;
        public Vector3 direction = Vector3.zero;

        private void Start()
        {
            // if(direction == Vector3.zero)
            //     direction = (GameManager.Instance.Player.transform.position - this.transform.position).normalized;
            this.rigidBody = this.GetComponent<Rigidbody>();

            Destroy(this.gameObject, timeToLive);
        }

        private void Update()
        {
            // TODO
            //this.rigidBody.velocity = 1.1f * ShipMovementHandler.TotalMaxSpeed * this.direction;
            this.rigidBody.velocity = speed * this.direction;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Scenery"))
            {
                if(other.TryGetComponent(out IDamageable health))
                {
                    health.TakeDamage(Damage);

                }
                Destroy(this.gameObject);
            }
        }
    }
}
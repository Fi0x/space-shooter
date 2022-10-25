using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Ship.Movement;
using UnityEngine;
using HealthSystem;
using Manager;

namespace Ship
{
    public class ShipCollision : MonoBehaviour
    {
        [SerializeField] private float damageMultiplier = 1.0f;
        private void OnCollisionEnter(Collision collision)
        {
            if(DoesCollide(collision.gameObject) && collision.relativeVelocity.magnitude > 10)
            {
                GameManager.Instance.CreateNewText("Crashing into things damages your ship!", 5, "collisionDamage");
                
                this.gameObject.GetComponent<IDamageable>().TakeDamage((int)collision.relativeVelocity.magnitude);
                if(this.gameObject.TryGetComponent(out PlayerShipMovementHandler shipMovementHandler))
                {
                    shipMovementHandler.NotifyAboutCollision();
                }
            }
        }

        private static bool DoesCollide(GameObject gameObject) =>  
            gameObject.layer == LayerMask.NameToLayer("Scenery")
                   || gameObject.layer == LayerMask.NameToLayer("Enemy")
                   || gameObject.layer == LayerMask.NameToLayer("Player");
    }
}

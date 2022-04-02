using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Ship.Movement;
using UnityEngine;
using HealthSystem;

namespace Ship
{
    public class ShipCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if(DoesCollide(collision.gameObject) && collision.relativeVelocity.magnitude > 10)
            {
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

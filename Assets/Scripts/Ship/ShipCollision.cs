using Components;
using Ship.Movement;
using UnityEngine;

namespace Ship
{
    public class ShipCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if(DoesCollide(collision.gameObject))
            {
                Debug.Log(collision.relativeVelocity.magnitude);
                if (collision.relativeVelocity.magnitude > 10)
                {
                    this.gameObject.GetComponent<Health>().TakeDamage((int)collision.relativeVelocity.magnitude);
                    if(this.gameObject.TryGetComponent(out PlayerShipMovementHandler shipMovementHandler))
                    {
                        shipMovementHandler.NotifyAboutCollision();
                    }
                }
            }
        }

        private static bool DoesCollide(GameObject gameObject) =>  
            gameObject.layer == LayerMask.NameToLayer("Scenery")
                   || gameObject.layer == LayerMask.NameToLayer("Enemy")
                   || gameObject.layer == LayerMask.NameToLayer("Player");
        
    }
}

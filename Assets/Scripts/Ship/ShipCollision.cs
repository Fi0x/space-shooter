using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Ship;

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
                this.gameObject.GetComponent<ShipMovementHandler>().NotifyAboutCollision();
            }
        }
    }

    private bool DoesCollide(GameObject gameObject)
    {
        if(gameObject.layer == LayerMask.NameToLayer("Scenery")
            || gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

public class ShipCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Scenery"))
        {
            Debug.Log(collision.relativeVelocity.magnitude);
            if (collision.relativeVelocity.magnitude > 10)
            {
                this.gameObject.GetComponent<Health>().TakeDamage((int)collision.relativeVelocity.magnitude);
            }
        }
    }
}

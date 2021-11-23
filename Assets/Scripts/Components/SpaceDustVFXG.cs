using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using UnityEngine.VFX;

public class SpaceDustVFXG : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private ShipMovementHandler smh;
    private Vector3 direction;

    void FixedUpdate()
    {
        var maxShipSpeed = this.smh.maxSpeed + this.smh.maxSpeedBoost;
        var shipSpeed = this.smh.shipRigidbody.velocity.magnitude;

        var value = (float)shipSpeed / maxShipSpeed;
        direction = Vector3.RotateTowards(direction, transform.InverseTransformDirection(smh.shipRigidbody.velocity.normalized),0.5f, 1.0f);
        vfx.SetVector3("Direction", direction);
        vfx.SetFloat("Speed", value);
    }
}

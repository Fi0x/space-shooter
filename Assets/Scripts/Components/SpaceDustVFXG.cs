using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using UnityEngine.VFX;

public class SpaceDustVFXG : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private ShipMovementHandler smh;

    void FixedUpdate()
    {
        var maxShipSpeed = this.smh.maxSpeed + this.smh.maxSpeedBoost;
        var shipSpeed = this.smh.shipRigidbody.velocity.magnitude;

        var value = (float)shipSpeed / maxShipSpeed;
        vfx.SetFloat("Speed", value);
    }
}

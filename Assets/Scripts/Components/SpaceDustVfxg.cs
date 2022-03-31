using System;
using UnityEngine;
using Ship;
using Ship.Movement;
using UnityEngine.VFX;

namespace Components
{
    public class SpaceDustVfxg : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;
        [SerializeField] private PlayerShipMovementHandler smh;



        private void FixedUpdate()
        {

            var velocity = this.smh.ShipRb.velocity;
            var speedPercent = this.smh.CurrentSpeed / this.smh.Settings.MaxSpeed;
            if (speedPercent < 0)
            {
                speedPercent = -speedPercent;
            }

            if (speedPercent > 1f)
            {
                speedPercent = 1;
            }

            this.vfx.SetVector3("Direction", this.transform.InverseTransformDirection(velocity.normalized));
            this.vfx.SetFloat("Speed", speedPercent);
        }
    }
}
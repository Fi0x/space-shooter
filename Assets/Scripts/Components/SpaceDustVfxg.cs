using System;
using UnityEngine;
using Ship;
using UnityEngine.VFX;

namespace Components
{
    public class SpaceDustVfxg : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;
        [SerializeField] private ShipMovementHandler2 smh;

        private float fractionToDisplay = 0.5f;

        private void HandleDesiredSpeedChangedEvent(float speed, float maxSpeed)
        {
            this.fractionToDisplay = speed / maxSpeed;
        }

        private void OnEnable()
        {
            this.smh.DesiredSpeedChangedEvent += this.HandleDesiredSpeedChangedEvent;
        }

        private void OnDisable()
        {
            this.smh.DesiredSpeedChangedEvent -= this.HandleDesiredSpeedChangedEvent;
        }

        private void FixedUpdate()
        {
            var velocity = this.smh.ShipRB.velocity;
            var speedPercent = this.fractionToDisplay;
            if (speedPercent < 0)
            {
                speedPercent = -speedPercent;
            }
            
            this.vfx.SetVector3("Direction", this.transform.InverseTransformDirection(velocity.normalized));
            this.vfx.SetFloat("Speed", speedPercent);
        }
    }
}
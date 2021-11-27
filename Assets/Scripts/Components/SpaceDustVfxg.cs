using UnityEngine;
using Ship;
using UnityEngine.VFX;

namespace Components
{
    public class SpaceDustVfxg : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;
        [SerializeField] private ShipMovementHandler smh;

        private void FixedUpdate()
        {
            var velocity = this.smh.shipRigidbody.velocity;
            var shipSpeed = velocity.magnitude;
            var speedPercent = shipSpeed / ShipMovementHandler.TotalMaxSpeed;
            
            this.vfx.SetVector3("Direction", this.transform.InverseTransformDirection(velocity.normalized));
            this.vfx.SetFloat("Speed", speedPercent);
        }
    }
}
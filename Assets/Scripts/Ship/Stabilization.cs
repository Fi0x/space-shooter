using System;
using UnityEngine;

namespace Ship
{
    public static class Stabilization
    {
        public static void StabilizeShip(ShipMovementHandler smh)
        {
            // Check if Speed exceeds max speed. if yes, clamp value down
            if (smh.shipRigidbody.velocity.magnitude > smh.maxSpeed)
            {
                smh.shipRigidbody.velocity = smh.shipRigidbody.velocity.normalized * smh.maxSpeed;
            }
            
            var vNow= smh.shipRigidbody.velocity;
            
            // Determine which sides need to trigger their thrusters
            // Local X
            var localX = smh.shipObject.transform.right;
            var dotProductCurrentDirectionXAxis = Vector3.Dot(localX, vNow);

#if DEBUG
            ShipMovementHandler.DotX = dotProductCurrentDirectionXAxis;
#endif

            if (!smh.isStrafing && Mathf.Abs(dotProductCurrentDirectionXAxis) > 0.05f)
            {
                float multiplier = smh.stabilizationMultiplier;
                if (dotProductCurrentDirectionXAxis > 0) multiplier = Math.Min(dotProductCurrentDirectionXAxis, multiplier);
                else multiplier = Math.Max(dotProductCurrentDirectionXAxis, -multiplier);
                smh.shipRigidbody.AddForce(multiplier * smh.accelerationLateral * -localX);
                if (dotProductCurrentDirectionXAxis > 0)
                {
                    // Need to move left (trigger right thrusters)
                    // TODO: Thruster Effect Management
                }
                else
                {
                    // Need to move right (trigger left thrusters)
                    // TODO: Thruster Effect Management
                }
            }

            // Local Y
            var localY = smh.shipObject.transform.up;
            var dotProductCurrentDirectionYAxis = Vector3.Dot(localY, vNow);
#if DEBUG
            ShipMovementHandler.DotY = dotProductCurrentDirectionYAxis;
#endif

            if (Mathf.Abs(dotProductCurrentDirectionYAxis) > 0.05f)
            {
                float multiplier = smh.stabilizationMultiplier;
                if (dotProductCurrentDirectionYAxis > 0) multiplier = Math.Min(dotProductCurrentDirectionYAxis, multiplier);
                else multiplier = Math.Max(dotProductCurrentDirectionYAxis, -multiplier);
                smh.shipRigidbody.AddForce(multiplier * smh.accelerationLateral * -localY);
                if (dotProductCurrentDirectionYAxis > 0)
                {
                    // Need to move left (trigger right thrusters)
                    // TODO: Thruster Effect Management
                }
                else
                {
                    // Need to move right (trigger left thrusters)
                    // TODO: Thruster Effect Management
                }
            }
            
            //TODO: Adjust forward speed towards desiredThrust
        }
    }
}
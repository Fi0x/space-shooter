using System;
using UnityEngine;

namespace Ship
{
    public static class Stabilization
    {
        public static float StabilizeShip(ShipMovementHandler smh, bool boosting)
        {
            // Check if Speed exceeds max speed. if yes, clamp value down
            //var topSpeed = smh.maxSpeed + (boosting ? smh.maxSpeedBoost : 0);
           // if (smh.shipRigidbody.velocity.magnitude > topSpeed)
            //{
            //    smh.shipRigidbody.velocity = smh.shipRigidbody.velocity.normalized * topSpeed;
           // }
            
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
                float multiplier = smh.stabilizationMultiplier * (boosting ? 5 : 1);
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
                float multiplier = smh.stabilizationMultiplier * (boosting ? 5 : 1);
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

            var localZ = smh.shipObject.transform.forward;
            return Vector3.Dot(localZ, vNow);
        }
    }
}
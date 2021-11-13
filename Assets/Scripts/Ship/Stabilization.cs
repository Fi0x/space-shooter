using System;
using UnityEngine;

namespace Ship
{
    public static class Stabilization
    {
        public static float StabilizeShip(ShipMovementHandler smh)
        {
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
                var multiplier = smh.stabilizationMultiplier * (smh.inputHandler.IsBoosting ? 5 : 1);
                multiplier = dotProductCurrentDirectionXAxis > 0
                    ? Math.Min(dotProductCurrentDirectionXAxis, multiplier)
                    : Math.Max(dotProductCurrentDirectionXAxis, -multiplier);
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
                var multiplier = smh.stabilizationMultiplier * (smh.inputHandler.IsBoosting ? 5 : 1);
                multiplier = dotProductCurrentDirectionYAxis > 0
                    ? Math.Min(dotProductCurrentDirectionYAxis, multiplier)
                    : Math.Max(dotProductCurrentDirectionYAxis, -multiplier);
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
            var dotProductCurrentDirectionZAxis = Vector3.Dot(localZ, vNow);

            return dotProductCurrentDirectionZAxis;
        }
    }
}
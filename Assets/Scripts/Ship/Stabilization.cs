using UnityEngine;

namespace Ship
{
    public class Stabilization
    {
        public static void StabilizeShip()
        {
            
        }
        public static void HandleStabilization(ShipMovementHandler smh, float maxSpeed, float accelerationLateral)
        {
            if (!smh.inputHandler.Strafing) ;
            // Check if Speed exceeds max speed. if yes, clamp value down
            if (smh.shipRigidbody.velocity.magnitude > maxSpeed)
            {
                smh.shipRigidbody.velocity = smh.shipRigidbody.velocity.normalized * maxSpeed;
            }


            var vNow = smh.shipRigidbody.velocity;

            // Determine which sides need to trigger their thrusters

            // Local X
            var localX = smh.shipObject.transform.right;
            var dotProductCurrentDirectionXAxis = Vector3.Dot(localX, vNow);

#if DEBUG
            ShipMovementHandler.dotX = dotProductCurrentDirectionXAxis;

#endif

            if (Mathf.Abs(dotProductCurrentDirectionXAxis) > 0.05f)
            {
                smh.shipRigidbody.AddForce(dotProductCurrentDirectionXAxis * accelerationLateral * -localX);
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
            ShipMovementHandler.dotY = dotProductCurrentDirectionYAxis;
#endif

            if (Mathf.Abs(dotProductCurrentDirectionYAxis) > 0.1f)
            {
                smh.shipRigidbody.AddForce(dotProductCurrentDirectionYAxis * accelerationLateral * -localY);
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
        }
    }
}
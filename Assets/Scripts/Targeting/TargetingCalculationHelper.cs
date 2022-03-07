#nullable enable
using System;
using UnityEngine;

namespace Targeting
{
    public static class TargetingCalculationHelper
    {
        
        public static float? GetPredictedTimeOfCollision(Vector3 shooterPosition, float projectileSpeed, Vector3 targetInitialPosition, Vector3 targetVelocity)
        {
            var deltaPosition = targetInitialPosition - shooterPosition;
            var ownMovement = targetVelocity;
            if (ownMovement.x == 0)
            {
                return null; // prevent NaN
            }

            var p = deltaPosition;
            var d = ownMovement;
            var v = projectileSpeed;

            var positionToDeltaPositionScalar = d.x * p.x + d.y * p.y + d.z * p.z;
            var leftTermInRoot = (-2 * positionToDeltaPositionScalar) * (-2 * positionToDeltaPositionScalar);
            var rightTermInRoot = 4 * (p.x * p.x + p.y * p.y + p.z * p.z) * (d.x * d.x + d.y * d.y + d.z * d.z - v * v);
            var termInRoot = leftTermInRoot - rightTermInRoot;

            if (termInRoot <= 0)
            {
                return null; // No solution.
            }
            
            var termOutsideRoot = 2 * positionToDeltaPositionScalar;
            var entireNumerator = Math.Sqrt(termInRoot) + termOutsideRoot;
            var entireDenominator = -2 * (d.x * d.x + d.y * d.y + d.z * d.z - v * v);


            var result = entireNumerator / entireDenominator;

            if (result <= 0)
            {
                return null;
            }

            return (float)result;
        }
    }
}
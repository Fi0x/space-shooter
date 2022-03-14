using System;
using System.Collections.Generic;
using UnityEngine;
using UpgradeNames = UpgradeSystem.Upgrades.UpgradeNames;

namespace Ship.Movement
{
    public abstract class ShipMovementHandlerBase : MonoBehaviour
    {
        public abstract ShipMovementHandlerSettings Settings { get; }
        
        protected abstract GameObject ShipObject { get; }
        public  abstract Rigidbody ShipRb { get; }

        public abstract Dictionary<Enum, int> Upgrades { get; }
        
        protected virtual void ModifyShipVector(Vector3 desiredVector)
        {
            var currentDirection = this.ShipRb.velocity;

            var currentDirectionLocalSpace = this.transform.InverseTransformDirection(currentDirection);
            var targetVectorLocalSpace = this.transform.InverseTransformDirection(desiredVector);

            var differenceCurrentDirectionToTargetLocalSpace = targetVectorLocalSpace - currentDirectionLocalSpace;

            this.HandleLateralThrust(
                differenceCurrentDirectionToTargetLocalSpace, targetVectorLocalSpace);

            // Update values
            currentDirectionLocalSpace = this.transform.InverseTransformDirection(currentDirection);
            differenceCurrentDirectionToTargetLocalSpace = targetVectorLocalSpace - currentDirectionLocalSpace;
            this.HandleMainThrust(differenceCurrentDirectionToTargetLocalSpace.z, targetVectorLocalSpace.z);
        }
        
        protected void HandleMainThrust(float deltaZLocalSpace, float zTargetLocalSpace, bool isBoosting = false)
        {
            if (!(Math.Abs(deltaZLocalSpace) > 0.1))
            {
                // Nothing to do. Z-Axis is within allowed Margin of Error.
                return;
            }
            var currentVelocityLocal = this.transform.InverseTransformDirection(this.ShipRb.velocity);
            var isBraking = Math.Abs(currentVelocityLocal.z) > Math.Abs(zTargetLocalSpace);

            if (deltaZLocalSpace > 0)
            {
                var effectiveAccelerationForce = this.Settings.AccelerationForwards(this.Upgrades[UpgradeNames.EngineAcceleration]);
                if (isBoosting)
                {
                    effectiveAccelerationForce *= this.Settings.AccelerationForwardsBoostMultiplier;
                }
                if (isBraking)
                {
                    effectiveAccelerationForce *= this.Settings.BrakingModifier(this.Upgrades[UpgradeNames.EngineStabilizationSpeed]);
                }
                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.ShipRb,
                    Vector3.forward * effectiveAccelerationForce * Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.z, zTargetLocalSpace,
                    velocityAfterForceLocal.z))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.z = zTargetLocalSpace;
                    this.ShipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
            else
            {
                var effectiveAccelerationForce = this.Settings.AccelerationBackwards(this.Upgrades[UpgradeNames.EngineDeceleration]);
                if (isBoosting)
                {
                    effectiveAccelerationForce *= this.Settings.AccelerationBackwardsBoostMultiplier;
                }
                if (isBraking)
                {
                    effectiveAccelerationForce *= this.Settings.BrakingModifier(this.Upgrades[UpgradeNames.EngineStabilizationSpeed]);
                }

                var velocityAfterForceLocal =
                    this.ModifyVelocityImmediateLocal(this.ShipRb, Vector3.back * effectiveAccelerationForce * Time.fixedDeltaTime);

                if (!IsValueInBetween(currentVelocityLocal.z, zTargetLocalSpace,
                    velocityAfterForceLocal.z))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.z = zTargetLocalSpace;
                    this.ShipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
        }

        protected virtual void HandleLateralThrust(Vector3 differenceCurrentDirectionToTargetLocalSpace, Vector3 targetVectorLocalSpace)
        {
            if (Math.Abs(differenceCurrentDirectionToTargetLocalSpace.x) > 0.1f)
            {
                this.HandleLateralX(differenceCurrentDirectionToTargetLocalSpace.x, targetVectorLocalSpace.x);
            }

            if (Math.Abs(differenceCurrentDirectionToTargetLocalSpace.y) > 0.1f)
            {
                this.HandleLateralY(differenceCurrentDirectionToTargetLocalSpace.y, targetVectorLocalSpace.y);
            }
        }

        protected virtual void HandleLateralX(float deltaXLocalSpace, float xTargetLocalSpace, bool boosting = false)
        {
            if (!(Math.Abs(deltaXLocalSpace) > 0.1))
            {
                // Nothing to do. X-Axis is within allowed Margin of Error.
                return;
            }
            var currentVelocityLocal = this.transform.InverseTransformDirection(this.ShipRb.velocity);

            var effectiveAccelerationLateral = this.Settings.AccelerationLateral(this.Upgrades[UpgradeNames.EngineLateralThrust]);
            if (boosting)
            {
                effectiveAccelerationLateral *= this.Settings.AccelerationLateralBoostMultiplier;
            }
            var isBraking = Math.Abs(currentVelocityLocal.x) > Math.Abs(xTargetLocalSpace);
            if (isBraking)
            {
                effectiveAccelerationLateral *= this.Settings.BrakingModifier(this.Upgrades[UpgradeNames.EngineStabilizationSpeed]);
            }

            if (deltaXLocalSpace > 0)
            {

                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.ShipRb,
                    Vector3.right * effectiveAccelerationLateral * Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.x, xTargetLocalSpace,
                    velocityAfterForceLocal.x))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.x = xTargetLocalSpace;
                    this.ShipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
            else
            {
                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.ShipRb, Vector3.left * effectiveAccelerationLateral *
                    Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.x, xTargetLocalSpace,
                    velocityAfterForceLocal.x))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.x = xTargetLocalSpace;
                    this.ShipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
        }
        
        protected virtual void HandleLateralY(float deltaYLocalSpace, float yTargetLocalSpace, bool boosting = false)
        {
            if (!(Math.Abs(deltaYLocalSpace) > 0.1))
            {
                // Nothing to do. Y-Axis is within allowed Margin of Error.
                return;
            }
            var currentVelocityLocal = this.transform.InverseTransformDirection(this.ShipRb.velocity);

            var effectiveAccelerationLateral = this.Settings.AccelerationLateral(this.Upgrades[UpgradeNames.EngineLateralThrust]);
            if (boosting)
            {
                effectiveAccelerationLateral *= this.Settings.AccelerationLateralBoostMultiplier;
            }
            var isBraking = Math.Abs(currentVelocityLocal.y) > Math.Abs(yTargetLocalSpace);
            if (isBraking)
            {
                effectiveAccelerationLateral *= this.Settings.BrakingModifier(this.Upgrades[UpgradeNames.EngineStabilizationSpeed]);
            }

            if (deltaYLocalSpace > 0)
            {
                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.ShipRb,
                    Vector3.up * effectiveAccelerationLateral * Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.y, yTargetLocalSpace,
                    velocityAfterForceLocal.y))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.y = yTargetLocalSpace;
                    this.ShipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
            else
            {
                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.ShipRb, Vector3.down * effectiveAccelerationLateral *
                    Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.y, yTargetLocalSpace,
                    velocityAfterForceLocal.y))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.y = yTargetLocalSpace;
                    this.ShipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
        }
        
        private Vector3 ModifyVelocityImmediateLocal(Rigidbody rb, Vector3 force)
        {
            var actualChange = this.transform.TransformDirection(force) / rb.mass;
            rb.velocity += actualChange;
            return this.transform.InverseTransformDirection(rb.velocity);
        }
        
        private static bool IsValueInBetween(float bound1, float bound2, float valueToCheck, bool boundsInclusive = false)
        {
            float lower, upper;
            if (bound1 < bound2)
            {
                lower = bound1;
                upper = bound2;
            }
            else
            {
                lower = bound2;
                upper = bound1;
            }

            if (boundsInclusive)
            {
                return valueToCheck >= lower && valueToCheck <= upper;
            }
            else
            {
                return valueToCheck > lower && valueToCheck < upper;
            }
        }
    }
}
#define DEBUG_GIZMO
using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UI;
using UnityEngine;

namespace Ship
{
    public class ShipMovementHandler : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private List<ShipMovementHandlerSettings> settings;
        [SerializeField] private int currentSettingsIndex = 0;

        [Header("Debug Data")] [SerializeField, ReadOnlyInspector]
        private float totalMaxSpeed;

        private bool isBoosting = false;

        private GameObject shipObject;
        private Rigidbody shipRb;
        private InputHandler inputHandler;
        private float desiredSpeed;
        public Rigidbody ShipRB => this.shipRb;
        public InputHandler InputHandler => this.inputHandler;

        public ShipMovementHandlerSettings Settings => this.settings[this.currentSettingsIndex];

        public float TotalMaxSpeed => this.totalMaxSpeed;
        public float CurrentSpeed { get; private set; } = 0;

        public event Action<ShipMovementHandlerSettings> SettingsUpdatedEvent;

        public event Action<float, float> DesiredSpeedChangedEvent;
        public event Action<bool> BoostingStateChangedEvent;

        /// <summary>
        /// This Event in invoked <b>after</b> forces have been applied onto the Ship's Rigidbody.
        /// The Camera uses this event to modify its position after forces.
        /// </summary>
        public event Action ForcesAppliedEvent;

        private void Start()
        {
            this.shipObject = this.gameObject;
            this.shipRb = this.shipObject.GetComponent<Rigidbody>();
            this.inputHandler = this.shipObject.GetComponent<InputHandler>();

            this.totalMaxSpeed = this.Settings.MaxSpeed + this.Settings.MaxSpeedBoost;

            LevelTransitionMenu.UpgradePurchasedEvent += (sender, args) =>
            {
                switch (args.Type)
                {
                    case LevelTransitionMenu.Upgrade.EngineAcceleration:
                        if(args.Increased) this.Settings.AccelerationForwards *= 1.1f;
                        else this.Settings.AccelerationForwards /= 1.1f;
                        break;
                    case LevelTransitionMenu.Upgrade.EngineDeceleration:
                        if(args.Increased) this.Settings.AccelerationBackwards *= 1.1f;
                        else this.Settings.AccelerationBackwards /= 1.1f;
                        break;
                    case LevelTransitionMenu.Upgrade.EngineLateralThrust:
                        if(args.Increased) this.Settings.AccelerationLateral *= 1.1f;
                        else this.Settings.AccelerationLateral /= 1.1f;
                        break;
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedPitch:
                        if(args.Increased) this.Settings.PitchSpeed *= 1.1f;
                        else this.Settings.PitchSpeed /= 1.1f;
                        break;
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedRoll:
                        if(args.Increased) this.Settings.RollSpeed *= 1.1f;
                        else this.Settings.RollSpeed /= 1.1f;
                        break;
                    case LevelTransitionMenu.Upgrade.EngingRotationSpeedYaw:
                        if(args.Increased) this.Settings.YawSpeed *= 1.1f;
                        else this.Settings.YawSpeed /= 1.1f;
                        break;
                    case LevelTransitionMenu.Upgrade.EngineStabilizationSpeed:
                        if(args.Increased) this.Settings.BrakingModifier *= 1.1f;
                        else this.Settings.BrakingModifier /= 1.1f;
                        break;
                    default:
                        return;
                }
                
                UpgradeMenuValues.InvokeUpgradeCompletedEvent(args);
            };
        }

        private void Awake()
        {
            // Check for nulls in List. This is a common error in serialized Lists.
            var withoutNulls = this.settings.Where(entry => entry != null).ToList();

            var nullCount = this.settings.Count - withoutNulls.Count;
            if (nullCount > 0)
            {
                Debug.LogError($"Found {nullCount} Null values. They have been removed for now");
                this.settings = withoutNulls;
            }
        }

        private void FixedUpdate()
        {
            var input = this.inputHandler.CurrentInputState;
            this.ApplyInputChanges(input);

            this.HandleAngularVelocity(input);
            this.ModifyShipVector(input);
            this.CurrentSpeed = this.ShipRB.velocity.magnitude;
            this.EffectiveForwardSpeed = this.transform.InverseTransformDirection(this.ShipRB.velocity).z;
            this.ForcesAppliedEvent?.Invoke();
        }

        public float EffectiveForwardSpeed { get; private set; }

        private void ApplyInputChanges(InputHandler.InputState input)
        {
            var oldDesiredSpeed = this.desiredSpeed;
            if (input.Braking)
            {
                this.desiredSpeed = 0f;
            }
            else
            {
                if (input.Boosting && false) // Disabled boost setting speed to max
                {
                    if (this.desiredSpeed < 0)
                    {
                        this.desiredSpeed = -this.TotalMaxSpeed;
                    }
                    else
                    {
                        this.desiredSpeed = this.TotalMaxSpeed;
                    }
                }
                else
                {
                    this.desiredSpeed += input.Thrust * this.Settings.MaxSpeed * 0.01f;

                    var maxSpeed = this.Settings.MaxSpeed;
                    if (this.desiredSpeed > maxSpeed)
                    {
                        // Clamp if at max speed
                        this.desiredSpeed = maxSpeed;
                    }
                    else if (this.desiredSpeed < -maxSpeed)
                    {
                        // Clamp if at max reverse speed
                        this.desiredSpeed = -maxSpeed;
                    }
                }


            }
            if (Math.Abs(this.desiredSpeed - oldDesiredSpeed) > 0.1)
            {
                this.DesiredSpeedChangedEvent?.Invoke(this.desiredSpeed, this.Settings.MaxSpeed);
            }

            if (this.isBoosting != input.Boosting)
            {
                this.isBoosting = input.Boosting;
                this.BoostingStateChangedEvent?.Invoke(this.isBoosting);
            }

            if (this.inputHandler.SwitchFlightModel)
            {
                this.inputHandler.SwitchFlightModel = false;
                this.HandleNewFlightModelSelected();
            }
        }

        private void HandleNewFlightModelSelected()
        {
            this.currentSettingsIndex = (this.currentSettingsIndex + 1) % this.settings.Count;
            this.HandleSettingsUpdatedEvent();
        }

        private void ModifyShipVector(InputHandler.InputState input)
        {
            var shipForward = this.shipObject.transform.forward;
            var targetVector = shipForward * this.desiredSpeed;
            if (input.Strafe != 0.0f)
            {
                targetVector += this.transform.TransformDirection(Vector3.right * input.Strafe * this.Settings.LateralMaxSpeed);
            }

            var currentDirection = this.shipRb.velocity;
#if DEBUG_GIZMO
            // Draws the Ships direction and target direction and the velocity components split up in global space
            Debug.DrawLine(this.shipRb.position, this.shipRb.position + currentDirection, Color.magenta);
            Debug.DrawLine(this.shipRb.position, this.shipRb.position + Vector3.right * currentDirection.x, Color.red);
            Debug.DrawLine(this.shipRb.position, this.shipRb.position + Vector3.up * currentDirection.y, Color.green);
            Debug.DrawLine(this.shipRb.position, this.shipRb.position + Vector3.forward * currentDirection.z, Color.blue);
            Debug.DrawLine(this.shipRb.position, this.shipRb.position + targetVector, Color.yellow);
#endif

            var currentDirectionLocalSpace = this.transform.InverseTransformDirection(currentDirection);
            var targetVectorLocalSpace = this.transform.InverseTransformDirection(targetVector);

            var differenceCurrentDirectionToTargetLocalSpace = targetVectorLocalSpace - currentDirectionLocalSpace;

            this.HandleLateralThrust(
                differenceCurrentDirectionToTargetLocalSpace, targetVectorLocalSpace);

            // Update values
            currentDirectionLocalSpace = this.transform.InverseTransformDirection(currentDirection);
            differenceCurrentDirectionToTargetLocalSpace = targetVectorLocalSpace - currentDirectionLocalSpace;
            this.HandleMainThrust(differenceCurrentDirectionToTargetLocalSpace.z, targetVectorLocalSpace.z);



        }

        private void HandleSettingsUpdatedEvent()
        {
            var currentSettings = this.Settings;
            this.totalMaxSpeed = currentSettings.MaxSpeed + currentSettings.MaxSpeedBoost;
            this.SettingsUpdatedEvent?.Invoke(currentSettings);
        }

        private void HandleMainThrust(float deltaZLocalSpace, float zTargetLocalSpace)
        {
            if (!(Math.Abs(deltaZLocalSpace) > 0.1))
            {
                // Nothing to do. Z-Axis is within allowed Margin of Error.
                return;
            }
            var currentVelocityLocal = this.transform.InverseTransformDirection(this.shipRb.velocity);
            var isBraking = Math.Abs(currentVelocityLocal.z) > Math.Abs(zTargetLocalSpace);

            if (deltaZLocalSpace > 0)
            {
                var effectiveAccelerationForce = this.Settings.AccelerationForwards;
                if (this.inputHandler.IsBoosting)
                {
                    effectiveAccelerationForce *= this.Settings.AccelerationForwardsBoostMultiplier;
                }
                if (isBraking)
                {
                    effectiveAccelerationForce *= this.Settings.BrakingModifier;
                }
                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.shipRb,
                    Vector3.forward * effectiveAccelerationForce * Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.z, zTargetLocalSpace,
                    velocityAfterForceLocal.z))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.z = zTargetLocalSpace;
                    this.shipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
            else
            {
                var effectiveAccelerationForce = this.Settings.AccelerationBackwards;
                if (this.inputHandler.IsBoosting)
                {
                    effectiveAccelerationForce *= this.Settings.AccelerationBackwardsBoostMultiplier;
                }
                if (isBraking)
                {
                    effectiveAccelerationForce *= this.Settings.BrakingModifier;
                }

                var velocityAfterForceLocal =
                    this.ModifyVelocityImmediateLocal(this.ShipRB, Vector3.back * effectiveAccelerationForce * Time.fixedDeltaTime);

                if (!IsValueInBetween(currentVelocityLocal.z, zTargetLocalSpace,
                    velocityAfterForceLocal.z))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.z = zTargetLocalSpace;
                    this.shipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
        }

        private void HandleLateralThrust(Vector3 differenceCurrentDirectionToTargetLocalSpace, Vector3 targetVectorLocalSpace)
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

        private void HandleLateralY(float deltaYLocalSpace, float yTargetLocalSpace)
        {
            if (!(Math.Abs(deltaYLocalSpace) > 0.1))
            {
                // Nothing to do. Y-Axis is within allowed Margin of Error.
                return;
            }
            var currentVelocityLocal = this.transform.InverseTransformDirection(this.shipRb.velocity);

            var effectiveAccelerationLateral = this.Settings.AccelerationLateral;
            if (this.inputHandler.IsBoosting)
            {
                effectiveAccelerationLateral *= this.Settings.AccelerationLateralBoostMultiplier;
            }
            var isBraking = Math.Abs(currentVelocityLocal.y) > Math.Abs(yTargetLocalSpace);
            if (isBraking)
            {
                effectiveAccelerationLateral *= this.Settings.BrakingModifier;
            }

            if (deltaYLocalSpace > 0)
            {
                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.shipRb,
                    Vector3.up * effectiveAccelerationLateral * Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.y, yTargetLocalSpace,
                    velocityAfterForceLocal.y))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.y = yTargetLocalSpace;
                    this.shipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
            else
            {
                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.ShipRB, Vector3.down * effectiveAccelerationLateral *
                    Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.y, yTargetLocalSpace,
                    velocityAfterForceLocal.y))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.y = yTargetLocalSpace;
                    this.shipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
        }

        private void HandleLateralX(float deltaXLocalSpace, float xTargetLocalSpace)
        {
            if (!(Math.Abs(deltaXLocalSpace) > 0.1))
            {
                // Nothing to do. X-Axis is within allowed Margin of Error.
                return;
            }
            var currentVelocityLocal = this.transform.InverseTransformDirection(this.shipRb.velocity);

            var effectiveAccelerationLateral = this.Settings.AccelerationLateral;
            if (this.inputHandler.IsBoosting)
            {
                effectiveAccelerationLateral *= this.Settings.AccelerationLateralBoostMultiplier;
            }
            var isBraking = Math.Abs(currentVelocityLocal.x) > Math.Abs(xTargetLocalSpace);
            if (isBraking)
            {
                effectiveAccelerationLateral *= this.Settings.BrakingModifier;
            }

            if (deltaXLocalSpace > 0)
            {

                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.shipRb,
                    Vector3.right * effectiveAccelerationLateral * Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.x, xTargetLocalSpace,
                    velocityAfterForceLocal.x))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.x = xTargetLocalSpace;
                    this.shipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
            else
            {
                var velocityAfterForceLocal = this.ModifyVelocityImmediateLocal(this.ShipRB, Vector3.left * effectiveAccelerationLateral *
                    Time.fixedDeltaTime);
                if (!IsValueInBetween(currentVelocityLocal.x, xTargetLocalSpace,
                    velocityAfterForceLocal.x))
                {
                    var newForceLocal = currentVelocityLocal;
                    newForceLocal.x = xTargetLocalSpace;
                    this.shipRb.velocity = this.transform.TransformDirection(newForceLocal);
                }
            }
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

        private void HandleAngularVelocity(InputHandler.InputState input)
        {
            var boosting = this.inputHandler.IsBoosting;

            var currentWorldAngularVelocity = this.shipRb.angularVelocity;
            var currentLocalAngularVelocity = this.shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

            // TODO: this is the wrong place to add this.
            var mouseMultiplier = InputManager.MouseSensitivity;

            var pitchForce = (boosting ? this.Settings.PitchSpeedBoostMultiplier : 1) * this.Settings.PitchSpeed;
            var yawForce = (boosting ? this.Settings.YawSpeedBoostMultiplier : 1) * this.Settings.YawSpeed;
            var rollForce = (boosting ? this.Settings.RollSpeedBoostMultiplier : 1) * this.Settings.RollSpeed;

            var effectivePitchForce = -input.Pitch * pitchForce * mouseMultiplier;
            var effectiveYawForce = input.Yaw * yawForce * mouseMultiplier;
            var effectiveRollForce = -input.Roll * rollForce;

            var angularForce = new Vector3(effectivePitchForce, effectiveYawForce, effectiveRollForce);
            currentLocalAngularVelocity += angularForce;

            var modifiedWorldAngularVelocity = this.shipObject.transform.TransformDirection(currentLocalAngularVelocity);
            this.shipRb.angularVelocity = modifiedWorldAngularVelocity;
        }

        public void SetNewTargetSpeed(int newSpeed)
        {
            this.desiredSpeed = newSpeed;
        }

        private Vector3 ModifyVelocityImmediate(Rigidbody rb, Vector3 force)
        {
            var actualChange = force / rb.mass;
            rb.velocity += actualChange;
            return rb.velocity;
        }

        private Vector3 ModifyVelocityImmediateLocal(Rigidbody rb, Vector3 force)
        {
            var actualChange = this.transform.TransformDirection(force) / rb.mass;
            rb.velocity += actualChange;
            return this.transform.InverseTransformDirection(rb.velocity);
        }
    }
}
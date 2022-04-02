//#define DEBUG_GIZMO

using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;
using UpgradeSystem;
using UpgradeNames = UpgradeSystem.UpgradeNames;

namespace Ship.Movement
{
    public class PlayerShipMovementHandler : ShipMovementHandlerBase
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
        public override Rigidbody ShipRb => this.shipRb;
        protected  override GameObject ShipObject => this.shipObject;
        
        public InputHandler InputHandler => this.inputHandler;

        public override ShipMovementHandlerSettings Settings => this.settings[this.currentSettingsIndex];

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
            var velocity = this.ShipRb.velocity;
            this.CurrentSpeed = velocity.magnitude;
            this.EffectiveForwardSpeed = this.transform.InverseTransformDirection(velocity).z;
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
            
        }


        private void ModifyShipVector(InputHandler.InputState input)
        {
            var shipForward = this.shipObject.transform.forward;
            var targetVector = shipForward * this.desiredSpeed;
            if (input.Strafe != 0.0f)
            {
                targetVector += this.transform.TransformDirection(Vector3.right * input.Strafe * this.Settings.LateralMaxSpeed);
            }
            
            base.ModifyShipVector(targetVector);
        }

        protected override void HandleLateralX(float deltaXLocalSpace, float xTargetLocalSpace, bool boosting = false) 
            => base.HandleLateralX(deltaXLocalSpace, xTargetLocalSpace, this.InputHandler.IsBoosting);

        protected override void HandleLateralY(float deltaXLocalSpace, float xTargetLocalSpace, bool boosting = false) 
            => base.HandleLateralY(deltaXLocalSpace, xTargetLocalSpace, this.InputHandler.IsBoosting);
        
        protected void HandleAngularVelocity(InputHandler.InputState input)
        {
            var boosting = this.inputHandler.IsBoosting;

            var currentWorldAngularVelocity = this.shipRb.angularVelocity;
            var currentLocalAngularVelocity = this.shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

            // TODO: this is the wrong place to add this.
            var mouseMultiplier = InputManager.MouseSensitivity;

            var pitchForce = (boosting ? this.Settings.PitchSpeedBoostMultiplier : 1) * this.Settings.PitchSpeed(upgradeData.GetValue(UpgradeNames.EngineHandling));
            var yawForce = (boosting ? this.Settings.YawSpeedBoostMultiplier : 1) * this.Settings.YawSpeed(upgradeData.GetValue(UpgradeNames.EngineHandling));
            var rollForce = (boosting ? this.Settings.RollSpeedBoostMultiplier : 1) * this.Settings.RollSpeed(upgradeData.GetValue(UpgradeNames.EngineHandling));

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

        // Collision
        public void NotifyAboutCollision()
        {
            this.desiredSpeed = 0;
        }
    }
}
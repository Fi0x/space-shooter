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
        private InputMap input;
        private float desiredSpeed;
        public override Rigidbody ShipRb => this.shipRb;
        protected override GameObject ShipObject => this.shipObject;

        //public InputMap Input => this.input;

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

        private void OnEnable()
        {
            input = new InputMap();
            input.Player.Enable();
        }

        private void OnDisable()
        {
            input.Player.Disable();
        }

        private void Start()
        {
            this.shipObject = this.gameObject;
            this.shipRb = this.shipObject.GetComponent<Rigidbody>();

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
            //var input = this.inputHandler.CurrentInputState;
            this.ApplyInputChanges();

            this.HandleAngularVelocity();
            this.ModifyShipVector();
            var velocity = this.ShipRb.velocity;
            this.CurrentSpeed = velocity.magnitude;
            this.EffectiveForwardSpeed = this.transform.InverseTransformDirection(velocity).z;
            this.ForcesAppliedEvent?.Invoke();
        }

        public float EffectiveForwardSpeed { get; private set; }

        private void ApplyInputChanges()
        {
            var inVal = this.input.Player.Acceleration.ReadValue<float>();
            if (inVal != 0)
                this.desiredSpeed = inVal > 0 ? this.Settings.MaxSpeed : this.Settings.MinSpeed;
            else
                this.desiredSpeed = 0;
        }


        private void ModifyShipVector()
        {
            var shipForward = this.shipObject.transform.forward;
            var targetVector = shipForward * this.desiredSpeed;
            var strafeInput = input.Player.LeftRight.ReadValue<float>();
            if (strafeInput != 0.0f)
            {
                targetVector += this.transform.TransformDirection(Vector3.right * strafeInput * this.Settings.LateralMaxSpeed);
            }

            base.ModifyShipVector(targetVector);
        }

        protected override void HandleLateralX(float deltaXLocalSpace, float xTargetLocalSpace, bool boosting = false)
            => base.HandleLateralX(deltaXLocalSpace, xTargetLocalSpace, input.Player.Boosting.WasPerformedThisFrame());

        protected override void HandleLateralY(float deltaXLocalSpace, float xTargetLocalSpace, bool boosting = false)
            => base.HandleLateralY(deltaXLocalSpace, xTargetLocalSpace, input.Player.Boosting.WasPerformedThisFrame());

        protected void HandleAngularVelocity()
        {
            var boosting = input.Player.Boosting.WasPerformedThisFrame();

            var currentWorldAngularVelocity = this.shipRb.angularVelocity;
            var currentLocalAngularVelocity = this.shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

            // TODO: this is the wrong place to add this.
            var mouseMultiplier = InputManager.MouseSensitivity;
            var turnInput = input.Player.Turn.ReadValue<Vector2>();

            var maxPitchForce = (boosting ? this.Settings.PitchSpeedBoostMultiplier : 1) * Settings.PitchSpeed(upgradeData.GetValue(UpgradeNames.EngineHandling));
            var maxYawForce = (boosting ? this.Settings.YawSpeedBoostMultiplier : 1) * Settings.YawSpeed(upgradeData.GetValue(UpgradeNames.EngineHandling));
            var maxRollForce = (boosting ? this.Settings.RollSpeedBoostMultiplier : 1) * Settings.RollSpeed(upgradeData.GetValue(UpgradeNames.EngineHandling));

            var pitchForce = -turnInput.y * mouseMultiplier;
            var yawForce = turnInput.x * mouseMultiplier;
            var rollForce = -input.Player.Roll.ReadValue<float>();

            var effectivePitchForce = Mathf.Clamp(pitchForce, -maxPitchForce, maxPitchForce);
            var effectiveYawForce = Mathf.Clamp(yawForce, -maxYawForce, maxYawForce);
            var effectiveRollForce = Mathf.Clamp(rollForce, -maxRollForce, maxRollForce);

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
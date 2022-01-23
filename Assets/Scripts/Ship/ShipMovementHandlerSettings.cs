using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ship
{
    [CreateAssetMenu(fileName = "ShipMovementHandlerDefaultSettings", menuName = "ScriptableObject/Ship/ShipMovementHandlerSettings", order = 50)]
    public class ShipMovementHandlerSettings : ScriptableObject
    {
        [SerializeField]
        private string profileName = "Uninitialized";
        [Header("Rotation Speed")]
        //
        [SerializeField] private float pitchSpeed = 1f;
        [SerializeField] private float rollSpeed = 2f;
        [SerializeField] private float yawSpeed = 0.8f;

        [Header("Movement Forces")]
        [SerializeField] private float accelerationForwards = 2f;
        [SerializeField] private float accelerationBackwards = 1f;
        [SerializeField] private float accelerationLateral = 1f;
        [SerializeField] private float maxLateralSpeed = 50f;
        [SerializeField] private float maxSpeed = 150f;
        [SerializeField] private float maxSpeedBoost = 75f;

        /**
         * To make movement feel more responsive, correctional forces can be stronger.
         *
         * The braking modifier is applied to the acceleration* values when the applied force would result in a shorter
         * velocity-Vector.
         */
        [Header("Braking Modifier")]
        [SerializeField] private float brakingModifier = 1f;

        /**
         * When the Boost is triggered, all forces are multiplied by the Boost Modifier. This can be used to make
         * certain thrusters stronger, while making other thrusters weaker.
         */
        [Header("Boost Modifiers")]
        [SerializeField] private float accelerationForwardsBoostMultiplier = 1f;
        [SerializeField] private float accelerationBackwardsBoostMultiplier = 1f;
        [SerializeField] private float accelerationLateralBoostMultiplier = 1f;
        [SerializeField] private float pitchSpeedBoostMultiplier = 1f;
        [SerializeField] private float rollSpeedBoostMultiplier = 2f;
        [SerializeField] private float yawSpeedBoostMultiplier = 0.8f;

        [FormerlySerializedAs("speedMatchDeadZone")] [SerializeField] private float correctionDeadZone = 0.01f;



        internal float PitchSpeed
        {
            get => this.pitchSpeed;
            set => this.pitchSpeed = value;
        }

        internal float RollSpeed
        {
            get => this.rollSpeed;
            set => this.rollSpeed = value;
        }

        internal float YawSpeed
        {
            get => this.yawSpeed;
            set => this.yawSpeed = value;
        }

        internal float AccelerationForwards
        {
            get => this.accelerationForwards;
            set => this.accelerationForwards = value;
        }

        internal float AccelerationBackwards
        {
            get => this.accelerationBackwards;
            private set => this.accelerationBackwards = value;
        }

        internal float AccelerationLateral
        {
            get => this.accelerationLateral;
            private set => this.accelerationLateral = value;
        }

        internal float MaxSpeed
        {
            get => this.maxSpeed;
            private set => this.maxSpeed = value;
        }

        internal float MaxSpeedBoost
        {
            get => this.maxSpeedBoost;
            private set => this.maxSpeedBoost = value;
        }

        internal float SpeedMatchDeadZone
        {
            get => this.correctionDeadZone;
            private set => this.correctionDeadZone = value;
        }

        internal string ProfileName
        {
            get => this.profileName;
            private set => this.profileName = value;
        }

        public float LateralMaxSpeed
        {
            get => this.maxLateralSpeed;
            private set => this.maxLateralSpeed = value;
        }

        public float MaxLateralSpeed => this.maxLateralSpeed;

        public float BrakingModifier => this.brakingModifier;

        public float AccelerationForwardsBoostMultiplier => this.accelerationForwardsBoostMultiplier;

        public float AccelerationBackwardsBoostMultiplier => this.accelerationBackwardsBoostMultiplier;

        public float AccelerationLateralBoostMultiplier => this.accelerationLateralBoostMultiplier;

        public float PitchSpeedBoostMultiplier => this.pitchSpeedBoostMultiplier;

        public float RollSpeedBoostMultiplier => this.rollSpeedBoostMultiplier;

        public float YawSpeedBoostMultiplier => this.yawSpeedBoostMultiplier;
    }
}
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
        [SerializeField]
        private float pitchSpeed = 1f;

        [SerializeField] private float rollSpeed = 2f;
        [SerializeField] private float yawSpeed = 0.8f;

        [Header("Movement Forces")]
        [SerializeField] private float accelerationForwards = 2f;
        [SerializeField] private float accelerationBackwards = 1f;
        [SerializeField] private float accelerationLateral = 1f;
        [SerializeField] private float maxLateralSpeed = 50f;
        [SerializeField] private float maxSpeed = 150f;
        [SerializeField] private float maxSpeedBoost = 75f;
        [SerializeField, Obsolete] private float minBrakeSpeed = 0.02f;
        [SerializeField, Obsolete] private float stabilizationMultiplier = 3;
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

        [Obsolete]
        internal float MinBrakeSpeed
        {
            get => this.minBrakeSpeed;
            private set => this.minBrakeSpeed = value;
        }

        [Obsolete]
        internal float StabilizationMultiplier
        {
            get => this.stabilizationMultiplier;
            private set => this.stabilizationMultiplier = value;
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
    }
}
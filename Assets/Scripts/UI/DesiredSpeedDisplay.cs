using Ship;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DesiredSpeedDisplay : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private ShipMovementHandler smh;
        [SerializeField] private Image indicator;
        [SerializeField] private float maxValue = 175;

        private InputHandler inputHandler;
        private Vector3 originalPosition;
        private float maxTotalSpeed;

        private void Start()
        {
            this.originalPosition = this.indicator.transform.localPosition;
            this.maxTotalSpeed = this.smh.maxSpeed + this.smh.maxSpeedBoost;
            this.inputHandler = this.smh.inputHandler;

            FlightModel.FlightModelChangedEvent += (sender, args) => { this.maxTotalSpeed = args.NewMaxSpeed + args.NewBoostSpeed; };
        }

        private void Update()
        {
            var thrustPercent = this.smh.desiredSpeed / this.maxTotalSpeed;
            var yOffset = this.maxValue * (this.inputHandler.IsBoosting ? 1 : thrustPercent);
            this.indicator.transform.localPosition = new Vector3(this.originalPosition.x, this.originalPosition.y + yOffset, this.originalPosition.z);
        }
    }
}
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

        private Vector3 originalPosition;
        private float maxTotalSpeed;

        private void Start()
        {
            this.originalPosition = this.indicator.transform.localPosition;
            this.maxTotalSpeed = this.smh.maxSpeed + this.smh.maxSpeedBoost;

            FlightModel.FlightModelChangedEvent += (sender, args) => { this.maxTotalSpeed = args.NewMaxSpeed + args.NewBoostSpeed; };
        }

        private void Update()
        {
            var thrustPercent = this.smh.desiredSpeed / this.maxTotalSpeed;
            this.indicator.transform.localPosition = new Vector3(this.originalPosition.x, this.originalPosition.y + thrustPercent * this.maxValue, this.originalPosition.z);
        }
    }
}
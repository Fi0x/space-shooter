using Ship;
using UI;
using UnityEngine;

namespace Components
{
    public class Speed : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private ShipMovementHandler smh;

        private SpeedIndicator speedIndicator;

        private void Start()
        {
            this.speedIndicator = this.GetComponentInParent<SpeedIndicator>();
            this.speedIndicator.SetMaxSpeed(this.smh.maxSpeed + this.smh.maxSpeedBoost);

            FlightModel.FlightModelChangedEvent += (sender, args) => { this.speedIndicator.SetMaxSpeed(args.NewMaxSpeed + args.NewBoostSpeed); };
        }

        private void Update()
        {
            this.speedIndicator.SetCurrentSpeed(this.smh.currentSpeed);
        }
    }
}
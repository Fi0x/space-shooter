using Ship;
using UI;
using UnityEngine;

namespace Components
{
    public class Speed : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private ShipMovementHandler2 smh;

        private SpeedIndicator speedIndicator;

        private void HandleDesiredSpeedChangedEvent(float speed, float maxSpeed)
        {
            this.speedIndicator.SetCurrentSpeed(speed);
        }

        private void OnEnable()
        {
            this.smh.DesiredSpeedChangedEvent += this.HandleDesiredSpeedChangedEvent;
        }

        private void OnDisable()
        {
            this.smh.DesiredSpeedChangedEvent -= this.HandleDesiredSpeedChangedEvent;
        }


        private void Start()
        {
            this.speedIndicator = this.GetComponentInParent<SpeedIndicator>();
            this.speedIndicator.SetMaxSpeed(this.smh.TotalMaxSpeed);

            FlightModel.FlightModelChangedEvent += (sender, args) => { this.speedIndicator.SetMaxSpeed(args.NewMaxSpeed + args.NewBoostSpeed); };
        }

        private void Update()
        {
           // this.speedIndicator.SetCurrentSpeed(this.smh.currentSpeed);
        }
    }
}
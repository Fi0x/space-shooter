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

        private void HandleDesiredSpeedChangedEvent(float speed, float maxSpeed)
        {
            this.speedIndicator.SetCurrentSpeed(speed);
        }

        private void OnEnable()
        {
            this.smh.DesiredSpeedChangedEvent += this.HandleDesiredSpeedChangedEvent;
            this.smh.SettingsUpdatedEvent += this.HandleSettingsUpdatedEvent;
        }

        private void HandleSettingsUpdatedEvent(ShipMovementHandlerSettings _)
        {
            this.speedIndicator.SetMaxSpeed(this.smh.TotalMaxSpeed);
        }

        private void OnDisable()
        {
            this.smh.DesiredSpeedChangedEvent -= this.HandleDesiredSpeedChangedEvent;
            this.smh.SettingsUpdatedEvent -= this.HandleSettingsUpdatedEvent;
        }


        private void Start()
        {
            this.speedIndicator = this.GetComponentInParent<SpeedIndicator>();
            this.speedIndicator.SetMaxSpeed(this.smh.TotalMaxSpeed);
        }

        private void Update()
        {
           this.speedIndicator.SetCurrentSpeed(this.smh.EffectiveForwardSpeed);
        }
    }
}
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
        private bool boostingStateLastFrame = false;

        private void HandleDesiredSpeedChangedEvent(float speed, float maxSpeed)
        {
            this.speedIndicator.SetCurrentSpeed(speed);
            if (this.boostingStateLastFrame != this.smh.InputHandler.IsBoosting)
            {
                this.boostingStateLastFrame = this.smh.InputHandler.IsBoosting;
                this.speedIndicator.SetBoostingState(this.boostingStateLastFrame);
            }
        }

        private void OnEnable()
        {
            this.smh.DesiredSpeedChangedEvent += this.HandleDesiredSpeedChangedEvent;
            this.smh.SettingsUpdatedEvent += this.HandleSettingsUpdatedEvent;
        }

        private void HandleSettingsUpdatedEvent(ShipMovementHandlerSettings _)
        {
            this.speedIndicator.SetMaxSpeed(this.smh.Settings.MaxSpeed);
        }

        private void OnDisable()
        {
            this.smh.DesiredSpeedChangedEvent -= this.HandleDesiredSpeedChangedEvent;
            this.smh.SettingsUpdatedEvent -= this.HandleSettingsUpdatedEvent;
        }


        private void Start()
        {
            this.speedIndicator = this.GetComponentInParent<SpeedIndicator>();
            this.speedIndicator.SetMaxSpeed(this.smh.Settings.MaxSpeed);
        }

        private void Update()
        {
           this.speedIndicator.SetCurrentSpeed(this.smh.EffectiveForwardSpeed);
        }
    }
}
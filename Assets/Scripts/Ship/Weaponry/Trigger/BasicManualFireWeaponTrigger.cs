#nullable enable
using System;
using Ship.Weaponry.Config;

namespace Ship.Weaponry.Trigger
{
    public class BasicManualFireWeaponTrigger : IWeaponTrigger
    {
        private float timeSinceLastShot = 0;
        private bool releasedAfterShot = true;
        private readonly WeaponConfigScriptableObject cfg;

        public BasicManualFireWeaponTrigger(WeaponConfigScriptableObject cfg)
        {
            this.cfg = cfg;
        }
        
        public void NotifyAboutTriggerStateChange(bool isPressedDown)
        {
            this.CurrentState = isPressedDown ? WeaponTriggerState.Firing : WeaponTriggerState.NotFiring; 
            if (isPressedDown && releasedAfterShot && timeSinceLastShot > this.TimeBetweenShots)
            {
                this.timeSinceLastShot = 0f;
                this.releasedAfterShot = false;
                
                this.WeaponFiredEvent?.Invoke();
            }
            else if (!isPressedDown)
            {
                this.releasedAfterShot = true;
            }
        }

        public event Action? WeaponFiredEvent;
        public WeaponTriggerState CurrentState { get; protected set; } = WeaponTriggerState.NotFiring;
        public float TimeBetweenShots => this.cfg.MinTimeBetweenShots * this.ShotDelayMultiplier;
        public float ShotDelayMultiplier { get; set; }

        public void Update(float dTime)
        {
            this.timeSinceLastShot += dTime;
        }
    }
}
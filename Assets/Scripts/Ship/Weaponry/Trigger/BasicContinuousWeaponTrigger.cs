using System;
using Ship.Weaponry.Config;
using UnityEngine;

namespace Ship.Weaponry.Trigger
{
    public class BasicContinuousWeaponTrigger : IWeaponTrigger
    {
        private readonly WeaponConfigScriptableObject config;

        private bool isFiring = false;
        
        public BasicContinuousWeaponTrigger(WeaponConfigScriptableObject cfg)
        {
            this.config = cfg;
            
            if (this.config.MinTimeBetweenShots != 0)
            {   
                Debug.LogWarning("MinTimeBetweenShots is set to a value != 0. Note that it is ignored");
            } 
        }

        public void NotifyAboutTriggerStateChange(bool isPressedDown)
        {
            if (isPressedDown == this.isFiring)
            {
                return;
            }

            this.isFiring = isPressedDown;
            this.CurrentState = isPressedDown ? WeaponTriggerState.Firing : WeaponTriggerState.NotFiring;
            this.StateChangedEvent?.Invoke(this.CurrentState);

        }

        public event Action WeaponFiredEvent;
        public event Action<float> WeaponFiredEventWithDeltaTime;
        public event Action<WeaponTriggerState> StateChangedEvent;
        public WeaponTriggerState CurrentState { get; private set; } = WeaponTriggerState.NotFiring;
        public float TimeBetweenShots => 0;
        public float ShotDelayUpgradeLevel { get; set; }

        public void Update(float dTime)
        {
            if (this.isFiring)
            {
                this.WeaponFiredEvent?.Invoke();
                this.WeaponFiredEventWithDeltaTime?.Invoke(dTime);
            }
        }
    }
}
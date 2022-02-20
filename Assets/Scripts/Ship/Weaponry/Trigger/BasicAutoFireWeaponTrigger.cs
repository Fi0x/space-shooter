#nullable enable
using System;
using Ship.Weaponry.Config;
using UnityEngine;

namespace Ship.Weaponry.Trigger
{
    public class BasicAutoFireWeaponTrigger : IWeaponTrigger
    {
        private float timeSinceLastShot = 0;
        private readonly WeaponConfigScriptableObject cfg;

        public BasicAutoFireWeaponTrigger(WeaponConfigScriptableObject cfg)
        {
            this.cfg = cfg;
        }

        public void NotifyAboutTriggerStateChange(bool isPressedDown)
        {
            this.CurrentState = isPressedDown ? WeaponTriggerState.Firing : WeaponTriggerState.NotFiring;
        }

        public event Action? WeaponFiredEvent;
        
        public WeaponTriggerState CurrentState { get; protected set; }
        public float TimeBetweenShots => this.cfg.MinTimeBetweenShots;

        public void Update(float dTime)
        {
            this.timeSinceLastShot += dTime;
            if (this.CurrentState == WeaponTriggerState.Firing)
            {
                if (this.timeSinceLastShot > this.TimeBetweenShots)
                {
                    this.timeSinceLastShot = 0f;
                    Debug.Log("Weapon Fired Event Triggered");
                    this.WeaponFiredEvent?.Invoke();
                }
            }
        }
    }
}
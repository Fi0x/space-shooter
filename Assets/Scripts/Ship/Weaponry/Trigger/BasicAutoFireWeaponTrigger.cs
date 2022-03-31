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

        public WeaponTriggerState CurrentState { get; protected set; } = WeaponTriggerState.NotFiring;
        public float TimeBetweenShots => this.cfg.MinTimeBetweenShots / (this.ShotDelayUpgradeLevel+1);
        public int ShotDelayUpgradeLevel { get; set; }

        public void Update(float dTime)
        {
            this.timeSinceLastShot += dTime;
            if (this.CurrentState == WeaponTriggerState.Firing)
            {
                if (this.timeSinceLastShot > this.TimeBetweenShots)
                {
                    this.timeSinceLastShot = 0f;
                    this.WeaponFiredEvent?.Invoke();
                }
            }
        }
    }
}
using System;

namespace Ship.Weaponry.Trigger
{
    public interface IWeaponTrigger
    {
        public void NotifyAboutTriggerStateChange(bool isPressedDown);

        public event Action WeaponFiredEvent;
        
        public WeaponTriggerState CurrentState { get; }
        float TimeBetweenShots { get; }
        int ShotDelayUpgradeLevel { get; set; }

        public void Update(float dTime);
    }
}
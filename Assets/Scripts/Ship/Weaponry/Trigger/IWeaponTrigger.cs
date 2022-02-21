using System;

namespace Ship.Weaponry.Trigger
{
    public interface IWeaponTrigger
    {
        public void NotifyAboutTriggerStateChange(bool isPressedDown);

        public event Action WeaponFiredEvent;
        
        public WeaponTriggerState CurrentState { get; }
        float TimeBetweenShots { get; }

        public void Update(float dTime);
    }
}
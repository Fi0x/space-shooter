#nullable enable
using Ship.Weaponry.Trigger;
using UnityEngine;

namespace Ship.Weaponry.Config
{
    public abstract class WeaponConfigScriptableObject : ScriptableObject
    {
        public enum WeaponTriggerType
        {
            SingleFire,
            AutoFire
        }

        [SerializeField] private WeaponTriggerType type;
        [SerializeField] private float minTimeBetweenShots;
        
        public WeaponTriggerType Type => this.type;
        public float MinTimeBetweenShots => this.minTimeBetweenShots;

        public virtual IWeaponTrigger? BuildWeaponTrigger()
        {
            switch (this.type)
            {
                case WeaponTriggerType.SingleFire:
                    return new BasicManualFireWeaponTrigger(this);
                case WeaponTriggerType.AutoFire:
                    return new BasicAutoFireWeaponTrigger(this);
                default:
                    // This is here to make overriding easier.
                    // The child class will not have to repeat the parent declarations.
                    return null;
            }
        }

        protected virtual void OnEnable()
        {
            // To nothing
        }
    }
}
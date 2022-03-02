using System;
using UnityEngine;

namespace Upgrades
{
    public class OldUpgradeButton : MonoBehaviour
    {
        //TODO: Remove class and use new system

        public static event EventHandler<UpgradePurchasedEventArgs> UpgradePurchasedEvent;
        
        public class UpgradePurchasedEventArgs : EventArgs
        {
            public readonly Upgrade Type;
            public readonly bool Increased;
        
            public UpgradePurchasedEventArgs(Upgrade upgradeType, bool increased)
            {
                this.Type = upgradeType;
                this.Increased = increased;
            }
        }
        
        public enum Upgrade
        {
            WeaponDamage,
            WeaponFireRate,
            WeaponProjectileSpeed,
            EngineAcceleration,
            EngineDeceleration,
            EngineLateralThrust,
            EngineRotationSpeedPitch,
            EngineRotationSpeedRoll,
            EngineRotationSpeedYaw,
            EngineStabilizationSpeed,
            Unknown
        }
    }
}
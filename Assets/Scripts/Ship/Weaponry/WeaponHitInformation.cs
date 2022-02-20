#nullable enable
using System;
using Ship.Sensors;

namespace Ship.Weaponry
{
    public class WeaponHitInformation
    {
        public WeaponType Type { get; }
        public float Damage { get; }
        public SensorTarget? Target { get; }

        public enum WeaponType
        {
            HitScan, Projectile, Rocket
        }

        public WeaponHitInformation(WeaponType weaponType, float damage, SensorTarget? target)
        {
            this.Type = weaponType;
            this.Damage = damage;
            this.Target = target;
        }

        public override string ToString()
        {
            return $"WeaponHit:: {Type.ToString()} - Dmg: {Math.Round(this.Damage, 2)} - Target: {Target?.ToString() ?? "null"}";
        }
    }
}
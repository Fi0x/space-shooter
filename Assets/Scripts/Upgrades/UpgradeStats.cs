namespace Upgrades
{
    public static class UpgradeStats
    {
        public static int WeaponDamageLevel { get; set; } = 1;
        public static int WeaponFireRateLevel { get; set; } = 1;
        public static int ProjectileVelocityLevel { get; set; } = 1;
        
        public static int ShipAccelerationLevel { get; set; } = 1;
        public static int ShipBrakeLevel { get; set; } = 1;
        public static int ShipLateralThrustLevel { get; set; } = 1;
        public static int ShipPitchSpeedLevel { get; set; } = 1;
        public static int ShipRollSpeedLevel { get; set; } = 1;
        public static int ShipYawSpeedLevel { get; set; } = 1;
        public static int ShipStabilizerLevel { get; set; } = 1;

        public static void Reset()
        {
            WeaponDamageLevel = 1;
            WeaponFireRateLevel = 1;
            ProjectileVelocityLevel = 1;
            
            ShipAccelerationLevel = 1;
            ShipBrakeLevel = 1;
            ShipLateralThrustLevel = 1;
            ShipPitchSpeedLevel = 1;
            ShipRollSpeedLevel = 1;
            ShipYawSpeedLevel = 1;
            ShipStabilizerLevel = 1;
        }

        public static int GetCurrentLevel(OldUpgradeButton.Upgrade type) => type switch
        {
            OldUpgradeButton.Upgrade.WeaponDamage => WeaponDamageLevel,
            OldUpgradeButton.Upgrade.WeaponFireRate => WeaponFireRateLevel,
            OldUpgradeButton.Upgrade.WeaponProjectileSpeed => ProjectileVelocityLevel,
            OldUpgradeButton.Upgrade.EngineAcceleration => ShipAccelerationLevel,
            OldUpgradeButton.Upgrade.EngineDeceleration => ShipBrakeLevel,
            OldUpgradeButton.Upgrade.EngineLateralThrust => ShipLateralThrustLevel,
            OldUpgradeButton.Upgrade.EngineRotationSpeedPitch => ShipPitchSpeedLevel,
            OldUpgradeButton.Upgrade.EngineRotationSpeedRoll => ShipRollSpeedLevel,
            OldUpgradeButton.Upgrade.EngineRotationSpeedYaw => ShipYawSpeedLevel,
            OldUpgradeButton.Upgrade.EngineStabilizationSpeed => ShipStabilizerLevel,
            OldUpgradeButton.Upgrade.Unknown => 0,
            _ => 0
        };
    }
}
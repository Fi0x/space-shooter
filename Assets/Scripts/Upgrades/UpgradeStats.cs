namespace Upgrades
{
    public static class UpgradeStats
    {
        public static int FreeUpgradePoints;

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
        
        public static int ArmorLevel { get; set; } = 1;

        public static void Reset()
        {
            //TODO:
        }

        public static int GetCurrentLevel(UpgradeButton.Upgrade type) => type switch
        {
            UpgradeButton.Upgrade.WeaponDamage => WeaponDamageLevel,
            UpgradeButton.Upgrade.WeaponFireRate => WeaponFireRateLevel,
            UpgradeButton.Upgrade.WeaponProjectileSpeed => ProjectileVelocityLevel,
            UpgradeButton.Upgrade.EngineAcceleration => ShipAccelerationLevel,
            UpgradeButton.Upgrade.EngineDeceleration => ShipBrakeLevel,
            UpgradeButton.Upgrade.EngineLateralThrust => ShipLateralThrustLevel,
            UpgradeButton.Upgrade.EngineRotationSpeedPitch => ShipPitchSpeedLevel,
            UpgradeButton.Upgrade.EngineRotationSpeedRoll => ShipRollSpeedLevel,
            UpgradeButton.Upgrade.EngineRotationSpeedYaw => ShipYawSpeedLevel,
            UpgradeButton.Upgrade.EngineStabilizationSpeed => ShipStabilizerLevel,
            UpgradeButton.Upgrade.Armor => ArmorLevel,
            UpgradeButton.Upgrade.Unknown => 0,
            _ => 0
        };
    }
}
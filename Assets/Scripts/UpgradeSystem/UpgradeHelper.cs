namespace UpgradeSystem
{
    public static class UpgradeHelper
    {
        public static string GetUpgradeCategory(UpgradeNames upgradeType) => upgradeType switch
        {
            UpgradeNames.WeaponDamage => "Weapons",
            UpgradeNames.WeaponFireRate => "Weapons",
            UpgradeNames.WeaponProjectileSpeed => "Weapons",

            UpgradeNames.EngineAcceleration => "Movement",
            UpgradeNames.EngineDeceleration => "Movement",
            UpgradeNames.EngineLateralThrust => "Movement",
            UpgradeNames.EngineRotationSpeedPitch => "Movement",
            UpgradeNames.EngineRotationSpeedRoll => "Movement",
            UpgradeNames.EngineRotationSpeedYaw => "Movement",
            UpgradeNames.EngineStabilizationSpeed => "Movement",

            UpgradeNames.Health => "Health",

            _ => "Unknown",
        };
    }
}
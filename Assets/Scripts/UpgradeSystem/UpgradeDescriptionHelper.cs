namespace UpgradeSystem
{
    public static class UpgradeDescriptionHelper
    {
        public static string GetDescriptionForUpgrade(UpgradeNames upgrade)
        {
            return upgrade switch
            {
                UpgradeNames.Health => "Increase you Ship's Health",
                UpgradeNames.EngineAcceleration =>
                    "Increase you Ship's Maneuverability by increasing how quick it can accelerate",
                UpgradeNames.EngineHandling => "Improve your Ship's Handling",
                UpgradeNames.MaxRockets => "Increase your Rocket Capacity",
                UpgradeNames.WeaponDamage => "Increase how much damage your weapons do",
                UpgradeNames.WeaponType => "Switch your Weapon",
                UpgradeNames.EngineStabilizationSpeed => 
                    "Improve your stabilization thrusters. Your ship will drift less",
                UpgradeNames.RocketChargeSpeed => "Improve how quick your Rockets Recharge",
                UpgradeNames.WeaponFireRate => "Improve the Firerate of your Gun",
                UpgradeNames.WeaponProjectileSpeed => "Improve how quick your projectiles shoot. Irrelevant for Lasers",
                _ => "TODO"
            };
        }
    }
}
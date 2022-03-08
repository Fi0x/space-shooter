#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using Ship.Weaponry;

namespace Stats
{
    public static class StatCollector
    {
        public static Dictionary<StatNames.StatValues, float> FloatStats { get; private set; } = null!;
        public static Dictionary<StatNames.StatValues, int> IntStats { get; private set; } = null!;

        public static Dictionary<WeaponHitInformation.WeaponType, float> WeaponTypeToDamageCausedStatLookup
        {
            get;
            private set;
        }

        public static void InitializeStatMaps()
        {
            FloatStats = new Dictionary<StatNames.StatValues, float>();
            IntStats = new Dictionary<StatNames.StatValues, int>();
            WeaponTypeToDamageCausedStatLookup = new Dictionary<WeaponHitInformation.WeaponType, float>();
        
            FloatStats.Add(StatNames.StatValues.DamageTaken, 0f);
            FloatStats.Add(StatNames.StatValues.DamageCaused, 0f);
        
            IntStats.Add(StatNames.StatValues.EnemiesKilled, 0);
            IntStats.Add(StatNames.StatValues.LevelsCompleted, 0);
            IntStats.Add(StatNames.StatValues.UpgradesPurchased, 0);

        
        }

        public static void NotifyAboutWeaponHit(WeaponHitInformation weaponHitInformation)
        {
            if (weaponHitInformation == null) throw new ArgumentNullException(nameof(weaponHitInformation));

            FloatStats[StatNames.StatValues.DamageCaused] += weaponHitInformation.Damage;
            if (!WeaponTypeToDamageCausedStatLookup.ContainsKey(weaponHitInformation.Type))
            {
                WeaponTypeToDamageCausedStatLookup[weaponHitInformation.Type] = 0;
            }
            WeaponTypeToDamageCausedStatLookup[weaponHitInformation.Type] += weaponHitInformation.Damage;
        }

        public static string? GetValueStringForStat(string statName)
        {
            var castedToEnum = StatNames.GetTypeFromDisplayName(statName);
            return GetValueStringForStat(castedToEnum);
        }

        private static string? GetValueStringForStat(StatNames.StatValues statValueAsEnum)
        {
            if (FloatStats.ContainsKey(statValueAsEnum))
                return FloatStats[statValueAsEnum].ToString(CultureInfo.InvariantCulture);
        
            return IntStats.ContainsKey(statValueAsEnum) ? IntStats[statValueAsEnum].ToString() : null;
        }

        public static void Reset()
        {
            InitializeStatMaps();
        }
    }
}
#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Ship.Weaponry;

public static class StatCollector
{
    public static Dictionary<StatValues, float> FloatStats { get; private set; } = null!;
    public static Dictionary<StatValues, int> IntStats { get; private set; } = null!;

    public static Dictionary<WeaponHitInformation.WeaponType, float> WeaponTypeToDamageCausedStatLookup
    {
        get;
        private set;
    }

    public static void InitializeStatMaps()
    {
        FloatStats = new Dictionary<StatValues, float>();
        IntStats = new Dictionary<StatValues, int>();
        WeaponTypeToDamageCausedStatLookup = new Dictionary<WeaponHitInformation.WeaponType, float>();
        
        FloatStats.Add(StatValues.DamageTaken, 0f);
        FloatStats.Add(StatValues.DamageCaused, 0f);
        
        IntStats.Add(StatValues.EnemiesKilled, 0);
        IntStats.Add(StatValues.LevelsCompleted, 0);
        IntStats.Add(StatValues.UpgradesPurchased, 0);

        
    }

    public static void NotifyAboutWeaponHit(WeaponHitInformation weaponHitInformation)
    {
        if (weaponHitInformation == null) throw new ArgumentNullException(nameof(weaponHitInformation));

        FloatStats[StatValues.DamageCaused] += weaponHitInformation.Damage;
        if (!WeaponTypeToDamageCausedStatLookup.ContainsKey(weaponHitInformation.Type))
        {
            WeaponTypeToDamageCausedStatLookup[weaponHitInformation.Type] = 0;
        }
        WeaponTypeToDamageCausedStatLookup[weaponHitInformation.Type] += weaponHitInformation.Damage;
    }

    public static string? GetValueStringForStat(string statName)
    {
        var castedToEnum = (StatValues)Enum.Parse(typeof(StatValues), statName);
        return GetValueStringForStat(castedToEnum);
        
       
    }

    private static string? GetValueStringForStat(StatValues statValueAsEnum)
    {
        if (FloatStats.ContainsKey(statValueAsEnum))
            return FloatStats[statValueAsEnum].ToString(CultureInfo.InvariantCulture);
        
        return IntStats.ContainsKey(statValueAsEnum) ? IntStats[statValueAsEnum].ToString() : null;
    }

    public static void Reset()
    {
        InitializeStatMaps();
    }
    
    public enum StatValues
    {
        DamageTaken,
        DamageCaused,
        EnemiesKilled,
        LevelsCompleted,
        UpgradesPurchased,
        TimePlayed
    }
}
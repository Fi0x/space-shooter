using System;
using System.Collections.Generic;

public static class StatCollector
{
    public static Dictionary<string, float> FloatStats { get; private set; }
    public static Dictionary<string, int> IntStats { get; private set; }

    public static void InitializeStatMaps()
    {
        FloatStats = new Dictionary<string, float>();
        IntStats = new Dictionary<string, int>();
        
        FloatStats.Add(nameof(StatValues.DamageTaken), 0);
        FloatStats.Add(nameof(StatValues.DamageCaused), 0);
        
        IntStats.Add(nameof(StatValues.EnemiesKilled), 0);
        IntStats.Add(nameof(StatValues.LevelsCompleted), 0);
        IntStats.Add(nameof(StatValues.UpgradesPurchased), 0);
    }
    
    public enum StatValues
    {
        DamageTaken,
        DamageCaused,
        EnemiesKilled,
        LevelsCompleted,
        UpgradesPurchased
    }
}
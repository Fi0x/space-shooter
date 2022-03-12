#nullable enable
using System.Collections.Generic;

namespace Stats
{
    public static class StatCollector
    {
        public static Dictionary<string, float> GeneralStats { get; } = new Dictionary<string, float>();
        public static Dictionary<string, float> WeaponStats { get; } = new Dictionary<string, float>();

        public static void UpdateGeneralStat(string statName, float valueDifference)
        {
            if(!GeneralStats.ContainsKey(statName))
                GeneralStats.Add(statName, 0f);
            var valueToStore = GeneralStats[statName] + valueDifference;
            GeneralStats[statName] = valueToStore;
        }
        public static void UpdateWeaponStat(string statName, float valueDifference)
        {
            if(!GeneralStats.ContainsKey(statName))
                GeneralStats.Add(statName, 0f);
            var valueToStore = GeneralStats[statName] + valueDifference;
            GeneralStats[statName] = valueToStore;
        }

        public static void ResetStats()
        {
            GeneralStats.Clear();
            WeaponStats.Clear();
        }
    }
}
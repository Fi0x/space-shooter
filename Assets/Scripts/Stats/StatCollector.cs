#nullable enable
using System.Collections.Generic;

namespace Stats
{
    public static class StatCollector
    {
        public static Dictionary<string, float> Stats { get; } = new Dictionary<string, float>();

        public static void UpdateStat(string statName, float valueDifference)
        {
            if(!Stats.ContainsKey(statName))
                Stats.Add(statName, 0f);
            var valueToStore = Stats[statName] + valueDifference;
            Stats[statName] = valueToStore;
        }

        public static void ResetStats()
        {
            Stats.Clear();
        }
    }
}
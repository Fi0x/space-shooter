using System;
using System.Text.RegularExpressions;

namespace Stats
{
    public class StatNames
    {
        public static string GetDisplayName(Enum type)
        {
            return Regex.Replace(type.ToString(), "(\\B[A-Z])", " $1");
        }

        public static StatValues GetTypeFromDisplayName(string displayName)
        {
            return (StatValues) Enum.Parse(typeof(StatValues), displayName.Replace(" ", ""));
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
}
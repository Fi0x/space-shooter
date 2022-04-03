namespace UpgradeSystem.CostAndGain
{
    public struct UpgradeData
    {
        public UpgradeData(int toLevel, uint cost, float fromValue, float toValue, string upgradeString)
        {
            this.FromLevel = toLevel - 1;
            this.ToLevel = toLevel;
            this.Cost = cost;
            this.FromValue = fromValue;
            this.ToValue = toValue;
            this.UpgradeString = upgradeString;
        }
        
        public int FromLevel { get; }
        public int ToLevel { get; }
        public uint Cost { get; }
        public float FromValue { get; }
        public float ToValue { get; }
        public string UpgradeString { get; } // eg "+12%" or "-30ms"
    }
}
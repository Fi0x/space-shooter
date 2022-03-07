using System;

namespace UpgradeSystem
{
    public interface IUpgradeable
    {
        void ResetUpgrades();
        void SetNewUpgradeValue(Enum type, int newLevel);
    }
}
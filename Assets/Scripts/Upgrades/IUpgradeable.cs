using System;

namespace Upgrades
{
    public interface IUpgradeable
    {
        void ResetUpgrades();
        void SetNewUpgradeValue(Enum type, int newLevel);
    }
}
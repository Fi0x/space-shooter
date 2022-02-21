namespace Ship.Weaponry.Trigger
{
    public interface IChargeableWeaponTrigger : IWeaponTrigger
    {
        public float ChargeState { get; }
    }
}
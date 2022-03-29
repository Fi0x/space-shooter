#nullable enable
using System;
using UnityEngine;

namespace Ship.Weaponry
{
    public class ContinuousLaserAdapter : HitScanLaserAdapter
    {
        [SerializeField] private HitScanWeapon weaponRef;
        
        public void Init(HitScanWeapon weaponRef)
        {
            this.weaponRef = weaponRef ?? throw new ArgumentNullException(nameof(weaponRef));
            this.NotifyAboutLaserStateChange(false);
        }

        public override void NotifyAboutLaserHittingTarget(float distance)
        {
            base.SetLength(distance);
            base.SetImpactAlpha(1f);
        }

        public override void NotifyAboutLaserNotHittingTarget()
        {
            base.SetLength(this.weaponRef.MaxLength);
            base.SetImpactAlpha(0f);
        }

        public override void NotifyAboutLaserStateChange(bool shooting)
        {
            base.SetImpactAlpha(shooting ? 1f : 0f);
            base.SetLength(shooting ? this.weaponRef.MaxLength : 0f);
            if (shooting)
            {
                this.visualEffect.Play();
            }
            else
            {
                this.visualEffect.Stop();
            }
        }
    }
}
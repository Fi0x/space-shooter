using System;
using UnityEngine;

namespace Ship.Weaponry
{
    public class ContinuousLaserAdapter : HitScanLaserAdapter
    {
        [SerializeField] private HitScanWeapon weaponRef;

        protected override void Start()
        {
            base.Start();
            if (this.weaponRef == null)
            {
                this.weaponRef = this.GetComponent<HitScanWeapon>() ?? 
                                 throw new Exception("Weapon Reference was not set");
            }
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
        }
    }
}
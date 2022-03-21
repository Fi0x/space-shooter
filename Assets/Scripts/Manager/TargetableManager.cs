#nullable enable
using System;
using System.Collections.Generic;
using Ship.Weaponry;
using Targeting;
using Targeting.TargetChoosingStrategy;
using UnityEngine;

namespace Manager
{
    public class TargetableManager
    {
        private PrimaryTargetChoosingHelper projectileTargetChoosingHelper;
        private PrimaryTargetChoosingHelper hitScanTargetChoosingHelper;
        
        
        public TargetableManager()
        {
            this.projectileTargetChoosingHelper =
                PrimaryTargetChoosingHelper.WithStrategy(new BasicProjectileTargetChoosingStrategy());
            this.hitScanTargetChoosingHelper = 
                PrimaryTargetChoosingHelper.WithStrategy(new BasicHitScanTargetChoosingStrategy());
        }


        private readonly Dictionary<Targetable, TargetableUIObject> targetables =
            new Dictionary<Targetable, TargetableUIObject>();
        

        public void NotifyAboutNewTargetable(Targetable targetable)
        {
            this.targetables.Add(targetable, targetable.UiElement);
        }

        public void NotifyAboutTargetableGone(Targetable targetable)
        {
            this.targetables.Remove(targetable);
            if (this.PrimaryTarget == targetable)
            {
                this.PrimaryTarget = null;
            }
        }

        public Targetable? RecalculatePrimaryTarget(WeaponManager playerWeaponManager)
        {

            var weapon = playerWeaponManager.PrimaryWeaponAttachmentPoint.Child;
            if (weapon is HitScanWeapon)
            {
                this.PrimaryTarget =
                    this.hitScanTargetChoosingHelper.GetTargetableToTarget(playerWeaponManager.transform, 
                        0f, this.PrimaryTarget);
            }
            else if (weapon is ProjectileWeapon projectileWeapon)
            {
                this.PrimaryTarget =
                    this.projectileTargetChoosingHelper.GetTargetableToTarget(playerWeaponManager.transform,
                        projectileWeapon.ProjectileSpeed, this.PrimaryTarget);
            }
            else
            {
                throw new NotImplementedException();
            }

            return this.PrimaryTarget;

        }

        public Targetable? PrimaryTarget { get; private set; } = null;

        public IEnumerable<Targetable> Targets => this.targetables.Keys;
    }
}
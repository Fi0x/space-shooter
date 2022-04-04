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

        private const float Interval = 0.5f;
        private float currentTime = 0;

        private readonly Dictionary<Targetable, TargetableUIObject> targetables =
            new Dictionary<Targetable, TargetableUIObject>();

        public Sprite TargetableInactiveSprite { get; }
        public Sprite TargetableActiveSprite { get; }

        public TargetableManager(Sprite targetableActive, Sprite targetableInactive)
        {
            this.projectileTargetChoosingHelper =
                PrimaryTargetChoosingHelper.WithStrategy(new BasicProjectileTargetChoosingStrategy());
            this.hitScanTargetChoosingHelper = 
                PrimaryTargetChoosingHelper.WithStrategy(new BasicHitScanTargetChoosingStrategy());

            this.TargetableActiveSprite = targetableActive;
            this.TargetableInactiveSprite = targetableInactive;
        }


        public void NotifyAboutNewTargetable(Targetable targetable)
        {
            this.targetables.Add(targetable, targetable.UiElement);
            Debug.Log($"Added Targetable to manager. Now has a total of {targetables.Count}");
        }

        public void NotifyAboutTargetableGone(Targetable targetable)
        {
            this.targetables.Remove(targetable);
            Debug.Log($"Removed Targetable from manager. Now has a total of {targetables.Count}");
            if (this.PrimaryTarget == targetable)
            {
                this.PrimaryTarget = null;
                var player = GameManager.Instance.Player;
                if (player is null)
                {
                    return;
                }
                this.RecalculatePrimaryTarget(GameManager.Instance.Player.GetComponent<WeaponManager>());
            }
        }

        public Targetable? RecalculatePrimaryTarget(WeaponManager playerWeaponManager)
        {
            var oldPrimaryTarget = this.PrimaryTarget;
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

            if (oldPrimaryTarget != this.PrimaryTarget)
            {
                if (oldPrimaryTarget != null)
                {
                    oldPrimaryTarget.NotifyAboutPrimaryTargetStateChange(false);
                }

                if (this.PrimaryTarget != null)
                {
                    this.PrimaryTarget.NotifyAboutPrimaryTargetStateChange(true);
                }
            }

            return this.PrimaryTarget;

        }

        public void NotifyAboutUpdate()
        {
            if(GameManager.Instance.Player == null) return;
            this.currentTime += Time.deltaTime;
            if (this.currentTime > Interval)
            {
                this.currentTime %= Interval;
                this.RecalculatePrimaryTarget(GameManager.Instance.Player.GetComponent<WeaponManager>());
            }
        }

        public Targetable? PrimaryTarget { get; private set; } = null;

        public IEnumerable<Targetable> Targets => this.targetables.Keys;
    }
}
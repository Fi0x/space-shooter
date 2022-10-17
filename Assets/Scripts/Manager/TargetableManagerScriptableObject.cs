#nullable enable
using System;
using System.Collections.Generic;
using Ship.Weaponry;
using Targeting;
using Targeting.TargetChoosingStrategy;
using UnityEngine;

namespace Manager
{
    
    [CreateAssetMenu(fileName = "TargetableManagerScriptableObject", menuName = "ScriptableObject/Gameplay/TargetingManager",
        order = 50)]
    public class TargetableManagerScriptableObject : ScriptableObject
    {
        private PrimaryTargetChoosingHelper projectileTargetChoosingHelper = PrimaryTargetChoosingHelper.WithStrategy(new BasicProjectileTargetChoosingStrategy());
        private PrimaryTargetChoosingHelper hitScanTargetChoosingHelper = PrimaryTargetChoosingHelper.WithStrategy(new BasicHitScanTargetChoosingStrategy());
        

        private const float Interval = 0.5f;
        private float currentTime = 0;

        private readonly Dictionary<Targetable, TargetableUIObject> targetables =
            new Dictionary<Targetable, TargetableUIObject>();

        [SerializeField] private Sprite targetableInactiveSprite;
        [SerializeField] private Sprite targetableActiveSprite;

        public Sprite TargetableInactiveSprite => targetableInactiveSprite;

        public Sprite TargetableActiveSprite => targetableActiveSprite;
        
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
                var player = GameManager.Instance.Player;
                if (player is null)
                {
                    return;
                }
                this.RecalculatePrimaryTarget(player.GetComponent<WeaponManager>());
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
                        0f, this.targetables.Keys ,this.PrimaryTarget);
            }
            else if (weapon is ProjectileWeapon projectileWeapon)
            {
                this.PrimaryTarget =
                    this.projectileTargetChoosingHelper.GetTargetableToTarget(playerWeaponManager.transform,
                        projectileWeapon.ProjectileSpeed, this.targetables.Keys, this.PrimaryTarget);
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
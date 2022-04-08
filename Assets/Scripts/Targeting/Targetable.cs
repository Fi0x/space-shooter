#nullable enable
using System;
using Manager;
using Ship.Weaponry;
using Ship.Weaponry.Config;
using UI.Ui3D;
using UnityEngine;

namespace Targeting
{
    public class Targetable : MonoBehaviour
    {
        [SerializeField, ReadOnlyInspector] private bool isPrimaryTarget = false;

        public virtual Vector3 Velocity => Vector3.zero;
        private TargetableUIObject? uiElement = null;

        public TargetableUIObject UiElement =>
            this.uiElement != null ? this.uiElement : throw new NullReferenceException("Ui Element is not set!");

        public bool IsPrimaryTarget => this.isPrimaryTarget;

        
        protected virtual void PreStart() {}
        protected virtual void PostStart() {}
        
        protected virtual void Start()
        {
            this.PreStart();
            if (this.uiElement == null)
            {
                Debug.Log("Before CreateUIElement");
                this.CreateUIElement();
            }

            GameManager.Instance.TargetableManager.NotifyAboutNewTargetable(this);
            this.PostStart();
        }

        private void CreateUIElement()
        {
            var instance = GameManager.Instance;
            if (instance == null)
            {
                Debug.LogError("EEE: Instance is null!");
                return;
            }
            var player = GameManager.Instance.Player;
            if (player == null)
            {
                Debug.LogError("Player was null. Did not create UI Element");
                return;
            }
            var manager = player.GetComponent<Ui3DManager>() ??
                          throw new NullReferenceException("No 3D UI Manager on the Player");
            var gameObjectToInstantiate = new GameObject("Targetable 3DUI");
            gameObjectToInstantiate.AddComponent<TargetableUIObject>();
            gameObjectToInstantiate.transform.parent = manager.UiRoot;
            var uiElementInstance = gameObjectToInstantiate.GetComponent<TargetableUIObject>();
            uiElementInstance.transform.localScale = Vector3.one * 0.6f;
            uiElementInstance.Init(this);
            this.uiElement = uiElementInstance;
        }

        protected virtual void OnDisable()
        {
            GameManager.Instance.TargetableManager.NotifyAboutTargetableGone(this);

            if (this.uiElement)
            {
                this.uiElement!.NotifyAboutParentBeingDestroyed();
            }
        }


        public (Vector3 position, float travelTime, bool canHit)? GetPredictedTargetLocation()
        {
            var player = GameManager.Instance.Player;
            if (player == null)
            {
                throw new NullReferenceException("Player not set");
            }

            var weapon = player.GetComponent<WeaponManager>().PrimaryWeaponAttachmentPoint.Child;
            return this.GetPredictedTargetLocation(player.transform.position, weapon);
        }
        

        public (Vector3 position, float travelTime, bool canHit)? GetPredictedTargetLocation(Vector3 shooterPosition,
            AbstractWeapon weapon)
        {
            if (weapon is HitScanWeapon hitScanWeapon)
            {
                var distanceToShooter = Vector3.Distance(shooterPosition, this.transform.position);
                
                return (this.gameObject.transform.position, 0f, (hitScanWeapon.WeaponConfig as WeaponHitScanConfigScriptableObject)!.MaxDistance > distanceToShooter);
            }
            else if (weapon is ProjectileWeapon projectileWeapon)
            {
                return this.GetPredictedTargetLocationProjectile(shooterPosition,
                    projectileWeapon.ProjectileSpeed, projectileWeapon.ProjectileTtl);
            }
            else
            {
                throw new Exception("Unsupported Weapon Config Type");
            }
        }

        private (Vector3 position, float travelTime, bool canHit)?  GetPredictedTargetLocationProjectile(Vector3 shooterPosition, float projectileSpeed, float ttl)
        {

            var timeOfCollision = this.GetPredictedTimeOfCollision(shooterPosition, projectileSpeed);
            if (timeOfCollision == null)
            {
                return null;
            }

            var ownMovement = this.Velocity;

            var position = this.transform.position + timeOfCollision.Value * ownMovement;
            return (position, timeOfCollision.Value, timeOfCollision.Value < ttl);
        }

        // Some black magic is happening here. Its pretty hard to get it from the code.
        // Please refer to this Desmos Page: https://www.desmos.com/calculator/jthl2vjkps
        private float? GetPredictedTimeOfCollision(Vector3 shooterPosition, float projectileSpeed)
        {
            if (float.IsNaN(this.Velocity.x) || this.Velocity.magnitude <= 0.01f)
            {
                return null;
            }
            return TargetingCalculationHelper.GetPredictedTimeOfCollision(shooterPosition, projectileSpeed,
                this.transform.position, this.Velocity);
        }

        public void NotifyAboutPrimaryTargetStateChange(bool isThisTargetablePrimaryTarget)
        {
            if (this.isPrimaryTarget != isThisTargetablePrimaryTarget)
            {
                this.isPrimaryTarget = isThisTargetablePrimaryTarget;
                this.PrimaryTargetStateChangeEvent?.Invoke(isThisTargetablePrimaryTarget);
            }
        }

        public event Action<bool>? PrimaryTargetStateChangeEvent;

    }
}
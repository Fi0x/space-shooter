#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Manager;
using Stats;
using Targeting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Ship.Weaponry
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private Transform target = null!;
        [SerializeField] private InputMap input;
        [SerializeField] private GameObject ship = null!;
        [SerializeField] private uint targetChangeCheckInterval = 10;
        [SerializeField] private float defaultConversionDistance = 50;
        [SerializeField, ReadOnlyInspector] private float debugWeaponConvergence = 0f;
        [SerializeField] private List<GameObject> possibleWeaponPrefabs = new List<GameObject>();
        
        private uint currentTargetChangeCheckInterval = 0;

        public Vector3 Target => this.target.position;

        public WeaponAttachmentPoint? PrimaryWeaponAttachmentPoint =>
            weaponAttachmentPoints.Count > 0 ? weaponAttachmentPoints[0] : null;

        private List<WeaponAttachmentPoint> weaponAttachmentPoints = new List<WeaponAttachmentPoint>();

        public IReadOnlyList<WeaponAttachmentPoint> WeaponAttachmentPoints => this.weaponAttachmentPoints;

        [SerializeField] private UnityEvent<WeaponHitInformation> enemyHitEvent = new UnityEvent<WeaponHitInformation>();
        public UnityEvent<WeaponHitInformation> EnemyHitEvent => this.enemyHitEvent;
        
        private bool isShooting = false;

        public event Action<bool>? FireModeChangedEvent;

        private void OnEnable()
        {
            this.enemyHitEvent.AddListener(HandleEnemyHitEvent);
            input = new InputMap();
            input.Player.Enable();
            input.Player.Shoot.performed += FirePressed;
            input.Player.Shoot.canceled += FireReleased;
        }

        private void OnDisable()
        {
            this.enemyHitEvent.RemoveListener(HandleEnemyHitEvent);
            input.Player.Shoot.performed -= FirePressed;
            input.Player.Shoot.canceled -= FireReleased;
        }

        private static void HandleEnemyHitEvent(WeaponHitInformation weaponHitInformation)
        {
            StatCollector.UpdateWeaponStat($"{weaponHitInformation.Type} Damage", weaponHitInformation.Damage);
        }
        
        private void Update()
        {
            if (currentTargetChangeCheckInterval++ >= this.targetChangeCheckInterval)
            {
                currentTargetChangeCheckInterval = 0;
                this.UpdatePrimaryTarget();
            }

            this.UpdateWeaponConvergence();
        }

        private void FirePressed(InputAction.CallbackContext ctx)
        {
            FireModeChangedEvent?.Invoke(true);
        }

        private void FireReleased(InputAction.CallbackContext ctx)
        {
            FireModeChangedEvent?.Invoke(false);
        }

        private void UpdateWeaponConvergence()
        {
            var weapon = this.PrimaryWeaponAttachmentPoint?.Child;
            if (weapon == null)
            {
                throw new NullReferenceException("Weapon is not set!");
            }
            
            var primaryTarget = GameManager.Instance?.TargetableManager.PrimaryTarget;
            
            var weaponConvergence = this.defaultConversionDistance;

            if (primaryTarget != null)
            {
                var result = primaryTarget.GetPredictedTargetLocation(this.ship.transform.position, weapon);
                if (result.HasValue)
                {
                    if (weapon.IsHitScan)
                    {
                        // Use the Target for convergence
                        weaponConvergence =
                            Vector3.Distance(primaryTarget.transform.position, this.ship.transform.position);
                    }
                    else
                    {
                        var projectileSpeed = ((weapon as ProjectileWeapon)!).ProjectileSpeed;
                        weaponConvergence = projectileSpeed * result.Value.travelTime;
                    }
                }
            }

            this.debugWeaponConvergence = weaponConvergence;

            this.target.transform.position = this.transform.position + this.transform.forward * weaponConvergence;
        }

        private void UpdatePrimaryTarget()
        {
            GameManager.Instance.TargetableManager.RecalculatePrimaryTarget(this);
        }

        public GameObject GetParentShipGameObject()
        {
            return this.ship;
        }

        public void NotifyAboutNewWeaponAttachmentPoint(WeaponAttachmentPoint weaponAttachmentPoint)
        {
            if (weaponAttachmentPoint == null) throw new ArgumentNullException(nameof(weaponAttachmentPoint));

            this.weaponAttachmentPoints.Add(weaponAttachmentPoint);
        }

        public GameObject GetWeaponForLevel(int level)
        {
            var idx = (level - 1) % this.possibleWeaponPrefabs.Count;
            return this.possibleWeaponPrefabs[idx];
        }

        public int CountWeaponTypes()
        {
            return this.possibleWeaponPrefabs.Count;
        }
    }
}
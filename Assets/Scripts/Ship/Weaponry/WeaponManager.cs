#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ship.Weaponry
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private Transform target = null!;
        [SerializeField] private InputHandler inputHandler = null!;
        [SerializeField] private GameObject ship = null!;

        public Vector3 Target => this.target.position;

        private List<WeaponAttachmentPoint> weaponAttachmentPoints = new List<WeaponAttachmentPoint>();

        public IReadOnlyList<WeaponAttachmentPoint> WeaponAttachmentPoints => this.weaponAttachmentPoints;

        [SerializeField] private UnityEvent<WeaponHitInformation> enemyHitEvent = new UnityEvent<WeaponHitInformation>();
        public UnityEvent<WeaponHitInformation> EnemyHitEvent => this.enemyHitEvent;
        
        private bool isShooting = false;

        public event Action<bool>? FireModeChangedEvent;

        private void OnEnable()
        {
            this.enemyHitEvent.AddListener(this.HandleEnemyHitEvent);
        }

        private void OnDisable()
        {
            this.enemyHitEvent.RemoveListener(this.HandleEnemyHitEvent);

        }

        private void HandleEnemyHitEvent(WeaponHitInformation weaponHitInformation)
        {
            // TODO: Add information about hit to statistics tracking here
            Debug.Log(weaponHitInformation.ToString());
        }

        private void Update()
        {
            if (this.isShooting == this.inputHandler.CurrentInputState.Shooting)
                return;

            this.isShooting = this.inputHandler.CurrentInputState.Shooting;
            
            this.FireModeChangedEvent?.Invoke(this.isShooting);
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
    }
}
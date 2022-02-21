#nullable enable
using System;
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
    }
}
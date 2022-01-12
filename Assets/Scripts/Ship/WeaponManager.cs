using UnityEngine;
using UnityEngine.Events;

namespace Ship
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private GameObject ship;

        public Vector3 Target => this.target.position;

        [SerializeField] private UnityEvent enemyHitEvent = new UnityEvent();
        public UnityEvent EnemyHitEvent => this.enemyHitEvent;
        public UnityEvent<bool> FireModeChangedEvent { get; } = new UnityEvent<bool>();
        private bool isShooting;

        private void Update()
        {
            if (this.isShooting == this.inputHandler.CurrentInputState.Shooting)
                return;

            this.isShooting = !this.isShooting;
            this.FireModeChangedEvent.Invoke(this.isShooting);
        }

        public GameObject GetParentShipGameObject()
        {
            return this.ship;
        }
    }
}
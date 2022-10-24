#nullable enable
using System;
using Ship.Movement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UpgradeSystem;

namespace Ship.Weaponry
{
    
    /**
     * This is a spot where a weapon can be added to the ship. Upon startup it notifies the parents
     * <see cref="WeaponManager"/> about its present.
     */
    public class WeaponAttachmentPoint : MonoBehaviour
    {
        [SerializeField] private string identifier = "WPN_Attachment_X";
        [SerializeField] private WeaponManager weaponManager = null!;
        [SerializeField] private UpgradeDataSO upgradeData;

        [SerializeField] public UnityEvent<float> WeaponFiredAndIsChargingEvent;

        private int currentWeaponIdx;
        private InputMap input;
        
        public WeaponManager WeaponManager => this.weaponManager;

        public AbstractWeapon? Child { get; private set; }


        private void Start()
        {
            if (this.weaponManager == null) throw new ArgumentNullException(nameof(this.weaponManager));
            this.weaponManager.NotifyAboutNewWeaponAttachmentPoint(this);
            this.Rebuild();
        }

        private void OnEnable()
        {
            this.input = new InputMap();
            this.input.Player.Enable();
            this.input.Player.NextWeapon.performed += this.ChangeWeapon;
        }

        private void OnDisable()
        {
            this.input.Player.Disable();
            this.input.Player.NextWeapon.performed -= this.ChangeWeapon;
        }

        private void Rebuild()
        {
            if (this.Child != null)
            {
                this.Child.Remove();
                // this.Child = null;
            }
            
            this.LoadWeapon();
        }

        //TODO: Call when mouse-wheel is scrolled
        private void ChangeWeapon(InputAction.CallbackContext ctx)
        {
            if(this.Child != null)
                Destroy(this.Child.gameObject);

            if (ctx.ReadValue<float>() > 0)
                this.currentWeaponIdx--;
            else
                this.currentWeaponIdx++;

            if (this.currentWeaponIdx < 0)
                this.currentWeaponIdx = this.WeaponManager.CountWeaponTypes() - 1;
            else if (this.currentWeaponIdx >= this.WeaponManager.CountWeaponTypes())
                this.currentWeaponIdx = 0;
            
            this.LoadWeapon();
        }

        private void LoadWeapon()
        {
            var correctPrefab = this.WeaponManager.GetWeaponForLevel(this.currentWeaponIdx);
            var newGameObject = Instantiate(correctPrefab, this.transform);

            this.Child = newGameObject.GetComponent<AbstractWeapon>() ?? throw new Exception(
                "Given Prefab is not a weapon (it does not have a Script that inherits from AbstractWeapon");
            this.NewWeaponBuiltEvent?.Invoke(this.Child);
            
            //TODO: Check if this needs to be unassigned when weapons get switched
            this.Child.OnInitEvent += weapon =>
            {
                weapon.WeaponTrigger.WeaponFiredEvent += () =>
                    this.WeaponFiredAndIsChargingEvent.Invoke(weapon.WeaponTrigger.TimeBetweenShots);
            };
        }

        public event Action<AbstractWeapon>? NewWeaponBuiltEvent;
    }
}

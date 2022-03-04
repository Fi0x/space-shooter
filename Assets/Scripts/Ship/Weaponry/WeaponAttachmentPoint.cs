#nullable enable
using System;
using UnityEngine;

namespace Ship.Weaponry
{
    
    /**
     * This is a spot where a weapon can be added to the ship. Upon startup it notifies the parents
     * <see cref="WeaponManager"/> about its present.
     */
    public class WeaponAttachmentPoint : MonoBehaviour
    {
        [SerializeField] private string identifier = "WPN_Attachment_X";
        [SerializeField] private GameObject weaponPrefab = null!;
        [SerializeField] private WeaponManager weaponManager = null!;

        private AbstractWeapon? child;
        public WeaponManager WeaponManager => this.weaponManager;


        private void Start()
        {
            if (this.weaponManager == null) throw new ArgumentNullException(nameof(this.weaponManager));
            this.weaponManager.NotifyAboutNewWeaponAttachmentPoint(this);
            this.Rebuild();
        }

        private void Rebuild()
        {
            if (this.child != null)
            {
                this.child.Remove();
                this.child = null;
            }

            var newGameObject = Instantiate(weaponPrefab, this.transform);

            this.child = newGameObject.GetComponent<AbstractWeapon>() ?? throw new Exception(
                "Given Prefab is not a weapon (it does not have a Script that inherits from AbstractWeapon");
            this.NewWeaponBuiltEvent?.Invoke(this.child);
        }

        public event Action<AbstractWeapon>? NewWeaponBuiltEvent;
    }
}

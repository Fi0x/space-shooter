#nullable enable
using System;
using Manager;
using Ship.Weaponry;
using UnityEngine;

namespace Targeting
{
    public class TargetUIRenderer : MonoBehaviour
    {
        [SerializeField] public WeaponAttachmentPoint weaponAttachment = null!;
        private AbstractWeapon? weapon;
        
        private void OnEnable()
        {
            if (this.weaponAttachment == null)
            {
                throw new NullReferenceException(nameof(this.weaponAttachment));
            }

            this.weaponAttachment.NewWeaponBuiltEvent += abstractWeapon => this.weapon = abstractWeapon;
        }

        private void Update()
        {
            
            if (weapon == null)
            {
                return;
            }
            
            foreach (var target in GameManager.Instance.TargetableManager.Targets)
            {
                var response = target.GetPredictedTargetLocation(this.weapon.transform.position, this.weapon);
                if (response == null)
                {
                    Debug.DrawRay(target.transform.position, Vector3.up, Color.red);
                    continue;
                }
                Debug.DrawLine(target.transform.position, response.Value.position, Color.green);
            }
        }
    }
}
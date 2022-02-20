using System;
using UnityEngine;

namespace Ship.Weaponry.Config
{
    [CreateAssetMenu(fileName = "WeaponConfigScriptableObject", menuName = "ScriptableObject/Ship/WeaponConfig/HitScan",
        order = 50)]
    public class WeaponHitScanConfigScriptableObject : WeaponConfigScriptableObject
    {
        [SerializeField] private float maxDistance;
        [SerializeField] private AnimationCurve damageOverDistanceNormalized = null!;
        [SerializeField] private GameObject hitScanEmitterPrefab = null!; // TODO: Yet to figure out how this would exactly work.

        public float MaxDistance => this.maxDistance;
        public AnimationCurve DamageOverDistanceNormalized => this.damageOverDistanceNormalized;
        public GameObject HitScanEmitterPrefab => this.hitScanEmitterPrefab;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _ = this.damageOverDistanceNormalized ?? throw new NullReferenceException(nameof(this.damageOverDistanceNormalized));
            //_ = (object)this.hitScanEmitterPrefab ?? throw new NullReferenceException(); TODO: 
        }
    }
}
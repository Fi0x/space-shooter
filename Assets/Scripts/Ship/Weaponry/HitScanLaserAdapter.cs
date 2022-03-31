using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Ship.Weaponry
{
    public abstract class HitScanLaserAdapter : MonoBehaviour
    {
        [SerializeField] protected VisualEffect visualEffect;

        protected virtual void Start()
        {
            if (this.visualEffect == null)
            {
                throw new Exception("Visual Effects of this Adapter are not set");
            }
        }

        protected void SetImpactAlpha(float alpha) => this.visualEffect.SetFloat("ImpactAlpha", alpha);

        protected virtual void SetLength(float length) => this.visualEffect.SetFloat("length", length);
        
        public abstract void NotifyAboutLaserHittingTarget(float distance);

        public abstract void NotifyAboutLaserNotHittingTarget();

        public abstract void NotifyAboutLaserStateChange(bool shooting);
    }
}

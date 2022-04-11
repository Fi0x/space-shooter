using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UI
{
    public class GrainVolumeScript : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        private FilmGrain filmGrain;
        private ColorAdjustments colorAdjustments;

        public void Start()
        {
            foreach (var entry in volume.sharedProfile.components)
            {
                if (entry is FilmGrain filmGrainEntry)
                {
                    this.filmGrain = filmGrainEntry;
                }
                else if (entry is ColorAdjustments colorAdjustmentsEntry)
                {
                    this.colorAdjustments = colorAdjustmentsEntry;
                }
            }
        }

        public void NotifyAboutNewGrainValue(float fraction)
        {
            var contrast = (fraction >= 1) ? -100 : -fraction * 50; 
            var saturation = -fraction * 100;
            var postExposure = -fraction * 5;
            
            this.filmGrain.intensity.Override(fraction);
            this.colorAdjustments.contrast.Override(contrast);
            this.colorAdjustments.saturation.Override(saturation);
            this.colorAdjustments.postExposure.Override(postExposure);
            
        }

        private void OnDisable()
        {
            this.filmGrain.intensity.Override(0f);
            this.colorAdjustments.contrast.Override(0f);
            this.colorAdjustments.saturation.Override(0f);
            this.colorAdjustments.postExposure.Override(0f);
        }
    }
}

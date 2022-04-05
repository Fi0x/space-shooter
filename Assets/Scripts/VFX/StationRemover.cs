using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace VFX
{
    public class StationRemover : MonoBehaviour
    {
        public List<Renderer> stationRenderers;
        public VisualEffect ringExplosion;
        public float timeFrame = 2f;
        public AnimationCurve dissolveCurve;

        private void Start()
        {
            //Explode();
        }

        public void Explode()
        {
            StartCoroutine(ExplosionSequence());
        }

        IEnumerator ExplosionSequence()
        {
            yield return new WaitForSeconds(2f);
            float elapsed = 0f;
            //start vfx
            ringExplosion.Play();
            yield return new WaitForSeconds(0.5f);
            List<Material> stationMats = new List<Material>();
            foreach (var render in stationRenderers)
            {
                stationMats.Add(render.material);
            }

            while (elapsed < timeFrame)
            {
                elapsed += Time.unscaledDeltaTime;
                var f = dissolveCurve.Evaluate(elapsed / timeFrame);
                foreach (var mat in stationMats)
                {
                    mat.SetFloat("_DissolveAmount", f);
                }
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}

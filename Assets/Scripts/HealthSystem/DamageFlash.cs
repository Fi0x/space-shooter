using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HealthSystem
{
    public class DamageFlash : MonoBehaviour
    {
        [Header("Feedback")]
        public AnimationCurve flashingCurve;
        public float flashingDuration;
        public List<Renderer> renderers;

        public void FlashOnce()
        {
            StartCoroutine(Flash(flashingDuration));
        }

        IEnumerator Flash(float time)
        {
            for (float t = 0f; t < time; t += Time.deltaTime)
            {
                foreach (var render in renderers)
                {
                    render.material.SetFloat("_FlashingStrength", flashingCurve.Evaluate(t / time));
                }
                yield return null;
            }
        }
    }
}
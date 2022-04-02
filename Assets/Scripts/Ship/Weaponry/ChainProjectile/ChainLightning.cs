using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Components;
using UnityEngine;
using UnityEngine.VFX;

namespace Ship.Weaponry
{
    public class ChainLightning : MonoBehaviour
    {
        [Header("Targeting")]
        public List<GameObject> targets;
        [SerializeField] private float strikeInterval;
        [SerializeField][ReadOnlyInspector]private Transform lastTarget;

        [Header("Damage")]
        public float damage;

        [Header("Dependencies")]
        [SerializeField] private LightningStrike lightning;
        [SerializeField] private RandomizeSound soundRandomizer;

        private void Awake()
        {
            lastTarget = this.gameObject.transform;
            Destroy(gameObject, 5f);
            //StrikeAll();
        }

        public void StrikeAll()
        {
            StartCoroutine(StrikeTargets());
        }

        IEnumerator StrikeTargets()
        {
            RemoveInvalidTargets();
            soundRandomizer.PlayRandomSound();
            if (targets.Count == 0)
            {
                StrikeSingleTarget(this.gameObject);
            }
            foreach (var target in targets)
            {
                StrikeSingleTarget(target);
                yield return new WaitForSeconds(strikeInterval);
            }
        }

        private void RemoveInvalidTargets()
        {
            List<GameObject> toRemove = new List<GameObject>();
            foreach (var target in targets)
            {
                if (target == null) toRemove.Add(target);
            }

            foreach (var r in toRemove)
            {
                targets.Remove(r);
            }
        }

        private void StrikeSingleTarget(GameObject target)
        {
            if(target == null) return;
            
            lightning.source = lastTarget;
            lightning.target = target.transform;
            lightning.Strike();

            if (target.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }

            lastTarget.position = target.transform.position;
        }
    }
}
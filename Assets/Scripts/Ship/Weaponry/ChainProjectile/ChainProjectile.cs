using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ship.Weaponry
{
    public class ChainProjectile : WeaponProjectile
    {
        [Header("Chain Settings")]
        [SerializeField] private float splashDamageMultiplier = 0.3f;
        [SerializeField] private float radius = 30f;
        [SerializeField] private int maxTargets = 5;
        [SerializeField] private LayerMask targetMask;

        protected override void HandleCollision(Collider other)
        {
            var targets = GetChainTargets();
            var transform1 = transform;
            var obj = Instantiate(impactPrefab, transform1.position, transform1.rotation);
            ChainLightning cl = obj.GetComponent<ChainLightning>();
            cl.targets = targets;
            
            var timeOnImpact = Time.timeAsDouble - this.startTime;
            cl.damage = (int) (DamageMultiplier * this.damageOverTimeNormalized.Evaluate((float) timeOnImpact / this.timeToLive)) * splashDamageMultiplier;
            cl.StrikeAll();
            //Destroy(gameObject);
            base.HandleCollision(other);
        }

        private List<GameObject> GetChainTargets()
        {
            var hits = Physics.OverlapSphere(transform.position, radius, targetMask).ToList();
            
            hits = hits.OrderBy(
                x => (this.gameObject.transform.position - x.gameObject.transform.position).sqrMagnitude
            ).ToList();
            if(maxTargets < hits.Count)
                hits.RemoveRange(maxTargets, hits.Count - maxTargets);

            return GetGameObjects(hits);
        }

        private List<GameObject> GetGameObjects(List<Collider> cList)
        {
            List<GameObject> newList = new List<GameObject>();
            foreach (var c in cList)
            {
                newList.Add(c.gameObject);
            }

            return newList;
        }
    }
}
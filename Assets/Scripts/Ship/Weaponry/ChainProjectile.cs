using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ship.Weaponry
{
    public class ChainProjectile : WeaponProjectile
    {
        [Header("Chain Settings")]
        [SerializeField] private float radius = 30f;
        [SerializeField] private int maxTargets = 5;

        private new void HandleCollision(Collider other)
        {
            Debug.Log(GetChainTargets());
        }

        private List<Collider> GetChainTargets()
        {
            var hits = Physics.OverlapSphere(transform.position, radius, LayerMask.NameToLayer("Enemy")).ToList();
            
            hits = hits.OrderBy(
                x => (this.gameObject.transform.position - x.gameObject.transform.position).sqrMagnitude
            ).ToList();
            hits.RemoveRange(maxTargets, hits.Count);

            return hits;
        }
    }
}
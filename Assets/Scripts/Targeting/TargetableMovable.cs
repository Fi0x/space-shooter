using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Targeting
{
    public class TargetableMovable : Targetable
    {
        [FormerlySerializedAs("shipRB")] [SerializeField] private Rigidbody shipRb = null!;

        protected override void PreStart()
        {
            if (this.shipRb == null)
            {
                this.shipRb = GetComponent<Rigidbody>() ??
                              throw new NullReferenceException("No Rigidbody set. Could not infer from GameObject.");
            }
        }

        public override Vector3 Velocity => this.shipRb.velocity;
    }
}
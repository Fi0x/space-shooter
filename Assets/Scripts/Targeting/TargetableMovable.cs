using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Targeting
{
    public class TargetableMovable : Targetable
    {
        [FormerlySerializedAs("shipRB")] [SerializeField] private Rigidbody shipRb = null!;

        protected override void OnEnable()
        {
            if (this.shipRb == null)
            {
                this.shipRb = GetComponent<Rigidbody>() ??
                              throw new NullReferenceException("No Rigidbody set. Could not infer from GameObject.");
            }
            base.OnEnable();
        }

        public override Vector3 Velocity => this.shipRb.velocity;
    }
}
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UpgradeSystem;

namespace Ship.Movement
{
    public class NpcShipMovementHandler : ShipMovementHandlerBase
    {
        [SerializeField] private ShipMovementHandlerSettings settings = null!;
        [SerializeField] private GameObject shipObject = null!;
        [SerializeField] private Rigidbody shipRb = null!;
        [SerializeField] private float rotationRate = 1;

        public override ShipMovementHandlerSettings Settings => this.settings;
        protected override GameObject ShipObject => this.shipObject;
        public override Rigidbody ShipRb => this.shipRb;

        private Vector3 desiredMovementVector = Vector3.zero;
        private Vector3? desiredLookAtWorldPosition = null;

        private void FixedUpdate()
        {
            this.HandleShipVector();
            this.HandleAngularVelocity();
        }

        private void HandleAngularVelocity()
        {
            var desiredLookAtDirection = this.GetDesiredLookAtDirection();
            var asQuaternion = Quaternion.LookRotation(desiredLookAtDirection, this.ShipObject.transform.up);
            var currentRotation = this.ShipObject.transform.rotation;

            var angleBetweenBothRotations = Quaternion.Angle(asQuaternion, currentRotation);
            if (angleBetweenBothRotations < 0.5f)
            {

                return;
            }

            var traversionRateThisTick = this.rotationRate * Time.fixedDeltaTime;
            var angleToTraverseThisTick = angleBetweenBothRotations > traversionRateThisTick
                ? traversionRateThisTick
                : angleBetweenBothRotations;
            this.ShipObject.transform.rotation = Quaternion.Slerp(currentRotation, asQuaternion, angleToTraverseThisTick/angleBetweenBothRotations);
        }
        
        private void HandleShipVector()
        {
            base.ModifyShipVector(this.desiredMovementVector);
        }

        private Vector3 GetDesiredLookAtDirection()
        {
            if (this.desiredLookAtWorldPosition == null)
            {
                return desiredMovementVector.normalized;
            }
            else
            {
                return (this.desiredLookAtWorldPosition - this.ShipObject.transform.position).Value.normalized;
            }
        }
        
        private void Start()
        {
            if (this.settings == null)
            {
                throw new NullReferenceException(nameof(settings));
            }

            // ReSharper disable ConstantNullCoalescingCondition
            this.shipObject ??= this.gameObject;
            this.shipRb ??= this.GetComponent<Rigidbody>() ?? throw new NullReferenceException(nameof(this.shipRb));
            // ReSharper enable ConstantNullCoalescingCondition
        }

        public void NotifyAboutNewTargetDirectionWithVelocity(Vector3 worldPositionDirectionWithVelocity)
        {
            this.desiredMovementVector = worldPositionDirectionWithVelocity;
        }

        public void NotifyAboutNewLookAtTarget(Vector3? worldPositionNullable)
        {
            this.desiredLookAtWorldPosition = worldPositionNullable;
        }
    }
}
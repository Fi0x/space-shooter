#nullable enable
using System;
using Ship.Movement;
using UnityEngine;

namespace Ship.Movement
{
    public class NpcShipMovementHandler : ShipMovementHandlerBase
    {
        public override ShipMovementHandlerSettings Settings { get; }
        protected override GameObject ShipObject { get; }
        public  override Rigidbody ShipRb { get; }
    }
}
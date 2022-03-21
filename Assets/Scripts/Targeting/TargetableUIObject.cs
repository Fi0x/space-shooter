#nullable enable
using System.Runtime.CompilerServices;
using UI.Ui3D;
using UnityEngine;

namespace Targeting
{
    public class TargetableUIObject : Ui3DElement
    {
        public Targetable Parent { get; private set; } = null!;

        public void Init(Targetable parent)
        {
            this.Parent = parent;
        }

        protected override Vector3? DesiredPosition => this.Parent.GetPredictedTargetLocation()?.position;
    }
}

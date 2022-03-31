using Ship.Weaponry;
using UnityEngine;

namespace UI.Ui3D
{
    public abstract class Ui3DElement : MonoBehaviour
    {
        public float ZOffset { get; } = 0f;
        
        protected abstract Vector3? DesiredPosition { get; }
        
        public void UpdateElement(Ui3DManager manager)
        {
            var desiredPos = this.DesiredPosition;
            
            if (desiredPos.HasValue)
            {
                // Direction in World Space
                var rootPosition = manager.UiRoot.position;
                var directionOfTarget = (desiredPos.Value - rootPosition).normalized;
                var offsetFromCentre = manager.DistanceToRoot + ZOffset;
                
                this.transform.position = rootPosition + directionOfTarget * offsetFromCentre;
                this.transform.rotation.SetLookRotation(rootPosition, manager.UiRoot.up);
                
            }
        }
    }
}
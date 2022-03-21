using Ship.Weaponry;
using UnityEngine;

namespace UI.Ui3D
{
    public abstract class Ui3DElement : MonoBehaviour
    {
        public float ZOffset { get; } = 0f;

        protected abstract Vector3? DesiredPosition { get; }
        
        public void UpdatePosition(Ui3DManager manager, float distanceToRoot)
        {
            if (this.DesiredPosition.HasValue)
            {
                // Direction in World Space
                var rootPosition = manager.UiRoot.position;
                this.transform.position = (DesiredPosition.Value - rootPosition).normalized *
                                          (manager.DistanceToRoot + ZOffset);
                this.transform.rotation.SetLookRotation(rootPosition, manager.UiRoot.up);
            }
        }
    }
}
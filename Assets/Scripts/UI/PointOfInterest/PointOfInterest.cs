using System;
using UnityEngine;

namespace UI
{
    public class PointOfInterest : MonoBehaviour
    {
        public static event Action<PointOfInterest> OnPoIAdded = delegate { };
    
        public static event Action<PointOfInterest> OnPoIRemoved = delegate { };

        private void Start()
        {
            Activate();
        }

        public void Activate()
        {
            OnPoIAdded(this);
        }

        public void Deactivate()
        {
            OnPoIRemoved(this);
        }

        private void OnDestroy()
        {
            Deactivate();
        }
    }
}

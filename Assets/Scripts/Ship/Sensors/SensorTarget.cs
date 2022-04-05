using System;
using UnityEngine;

namespace Ship.Sensors
{
    public class SensorTarget : MonoBehaviour
    {
        public delegate void TargetDestroyed(SensorTarget target);

        [SerializeField] private TargetAllegiance allegiance;
        [SerializeField] private TargetType targetType;

        public TargetAllegiance Allegiance => this.allegiance;
        public TargetType Type => this.targetType;
        public Vector3 Position => this.gameObject.transform.position;
        
        public static event Action<SensorTarget> OnSensorTargetAdded = delegate { };

        public event TargetDestroyed TargetDestroyedEvent;
        public event EventHandler AllegianceChangedEvent;

        public void UpdateAllegiance(TargetAllegiance newAllegiance)
        {
            if(newAllegiance == this.allegiance)
                return;
            
            this.allegiance = newAllegiance;
            this.AllegianceChangedEvent?.Invoke(null, null);
        }

        private void Start()
        {
            OnSensorTargetAdded(this);
        }

        private void OnDestroy()
        {
            this.TargetDestroyedEvent?.Invoke(this);
        }

        public enum TargetAllegiance
        {
            Friendly,
            Neutral,
            Hostile
        }

        public enum TargetType
        {
            Ship,
            BigShip,
            Station,
            Missile,
            JumpGate
        }
    }
}

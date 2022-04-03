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

        private void Start()
        {
            OnSensorTargetAdded(this);
        }

        private void OnDestroy()
        {
            this.TargetDestroyedEvent?.Invoke(this);
        }

        public void ChangeAllegiance(TargetAllegiance allegiance)
        {
            Debug.Log("SensorTarget.ChangeType");
            this.allegiance = allegiance;
        }

        public enum TargetAllegiance
        {
            Friendly,
            Neutral,
            Hostile,
            JumpGate
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

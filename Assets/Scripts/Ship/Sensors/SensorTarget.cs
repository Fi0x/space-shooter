using System;
using UnityEngine;

namespace Ship.Sensors
{
    public class SensorTarget : MonoBehaviour
    {
        public delegate void TargetDestroyed(SensorTarget target);

        public enum TargetAllegiance
        {
            Friendly, Neutral, Hostile
        }

        public enum TargetType
        {
            Ship, Station, Missile
        }


        [SerializeField] [ReadOnlyInspector] private TargetAllegiance allegiance;
        [SerializeField] [ReadOnlyInspector] private TargetType targetType;
        [SerializeField] [ReadOnlyInspector] private bool isInit = false;

        public TargetAllegiance Allegiance => this.allegiance;
        public TargetType Type => this.targetType;
        public Vector3 Position => this.gameObject.transform.position;

        public event TargetDestroyed TargetDestroyedEvent;

        public void Init(TargetType targetType, TargetAllegiance targetAllegiance)
        {
            if (isInit)
            {
                throw new Exception("Script already initialized");
            }

            isInit = true;

            this.targetType = targetType;
            this.allegiance = targetAllegiance;
        }

        private void OnDestroy()
        {
            this.TargetDestroyedEvent?.Invoke(this);
        }
    }
}
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Ship.Sensors
{
    public class SensorObject : MonoBehaviour
    {

        [SerializeField] [ReadOnlyInspector] private SensorTarget target;
        [SerializeField] [ReadOnlyInspector] private bool isInit = false;
        [SerializeField] [ReadOnlyInspector] private RadarManager radarManager;

        [SerializeField] [ReadOnlyInspector] private LineRenderer lineRenderer;
        [SerializeField] [ReadOnlyInspector] private GameObject spriteObject;

        public delegate void SensorObjectDestroyed(SensorObject sensorObject);

        public event SensorObjectDestroyed SensorObjectDestroyedEvent;

        private void Start()
        {
            this.lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
            this.lineRenderer.positionCount = 2;
        }

        public void Init(SensorTarget target, RadarManager radarManager)
        {
            if (this.isInit)
            {
                throw new Exception("Script already Initialized");
            }
            this.radarManager = radarManager;
            this.spriteObject = this.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject;
            var sr = this.spriteObject.GetComponent<SpriteRenderer>();
            sr.color = target.Allegiance switch
            {
                SensorTarget.TargetAllegiance.Friendly => this.radarManager.ColorFriendly,
                SensorTarget.TargetAllegiance.Neutral => this.radarManager.ColorNeutral,
                SensorTarget.TargetAllegiance.Hostile => this.radarManager.ColorHostile,
                _ => throw new Exception("Unexpected Sensor Target Allegiance")
            };

            sr.sprite = target.Type switch
            {
                SensorTarget.TargetType.Missile => this.radarManager.SpriteMissile,
                SensorTarget.TargetType.Ship => this.radarManager.SpriteShip,
                SensorTarget.TargetType.Station => this.radarManager.SpriteStation,
                _ => throw new Exception("Unexpected Sensor Target Type")
            };
            this.spriteObject.transform.LookAt(Camera.main.transform);

            this.target = target;
            this.target.TargetDestroyedEvent += OnSensorTargetDestroyedEventHandler;

            this.isInit = true;

        }

        private void OnSensorTargetDestroyedEventHandler(SensorTarget _)
        {
            Destroy(this.gameObject);
            this.SensorObjectDestroyedEvent?.Invoke(this);
        }


        // Update is called once per frame
        private void Update()
        {
            var newLocalPosition = this.radarManager.ApplyPositionTransformation(this.target.Position);
            var newLocalPositionFloor = new Vector3(newLocalPosition.x, 0, newLocalPosition.z);
            this.spriteObject.transform.localPosition = newLocalPosition;
            var linePoints = new[]
                {newLocalPosition, newLocalPositionFloor};
            this.lineRenderer.SetPositions(linePoints);
        }
    }
}

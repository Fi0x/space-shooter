using System;
using UnityEngine;

namespace Ship.Sensors
{
    public class SensorObject : MonoBehaviour
    {

        [SerializeField] [ReadOnlyInspector] private SensorTarget target;
        [SerializeField] [ReadOnlyInspector] private bool isInit;
        [SerializeField] [ReadOnlyInspector] private RadarManager radarManager;

        [SerializeField] [ReadOnlyInspector] private LineRenderer lineRenderer;
        [SerializeField] [ReadOnlyInspector] private GameObject spriteObject;

        private void Start()
        {
            this.lineRenderer = this.gameObject.GetComponentInChildren<LineRenderer>();
            this.lineRenderer.positionCount = 2;
        }

        public void Init(SensorTarget st, RadarManager rm)
        {
            if (this.isInit)
            {
                throw new Exception("Script already Initialized");
            }
            this.radarManager = rm;
            this.spriteObject = this.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject;
            var sr = this.spriteObject.GetComponent<SpriteRenderer>();
            sr.color = st.Allegiance switch
            {
                SensorTarget.TargetAllegiance.Friendly => this.radarManager.ColorFriendly,
                SensorTarget.TargetAllegiance.Neutral => this.radarManager.ColorNeutral,
                SensorTarget.TargetAllegiance.Hostile => this.radarManager.ColorHostile,
                SensorTarget.TargetAllegiance.JumpGate => this.radarManager.JumpGateColor,
                _ => throw new Exception("Unexpected Sensor Target Allegiance")
            };

            sr.sprite = st.Type switch
            {
                SensorTarget.TargetType.Ship => this.radarManager.SpriteShip,
                SensorTarget.TargetType.Station => this.radarManager.SpriteStation,
                SensorTarget.TargetType.Missile => this.radarManager.SpriteMissile,
                SensorTarget.TargetType.JumpGate => this.radarManager.SpriteJumpGate,
                _ => throw new Exception("Unexpected Sensor Target Type")
            };
            var mainCam = Camera.main;
            if(mainCam) this.spriteObject.transform.LookAt(mainCam.transform);

            this.target = st;
            this.target.TargetDestroyedEvent += this.OnSensorTargetDestroyedEventHandler;

            this.isInit = true;
        }

        private void OnSensorTargetDestroyedEventHandler(SensorTarget _)
        {
            Destroy(this.gameObject);
        }
        
        private void LateUpdate()
        {
            UpdateSprite();
            var newLocalPosition = this.radarManager.ApplyPositionTransformation(this.target.Position);
            var newLocalPositionFloor = new Vector3(newLocalPosition.x, 0, newLocalPosition.z);
            this.spriteObject.transform.localPosition = newLocalPosition;
            var linePoints = new[] {newLocalPosition, newLocalPositionFloor};
            this.lineRenderer.SetPositions(linePoints);
        }

        private void OnDisable()
        {
            this.target.TargetDestroyedEvent -= this.OnSensorTargetDestroyedEventHandler;
        }

        public void UpdateSprite()
        {
            SpriteRenderer spriteRenderer = this.spriteObject.GetComponent<SpriteRenderer>();

            spriteRenderer.color = target.Allegiance switch
            {
                SensorTarget.TargetAllegiance.Friendly => this.radarManager.ColorFriendly,
                SensorTarget.TargetAllegiance.Neutral => this.radarManager.ColorNeutral,
                SensorTarget.TargetAllegiance.Hostile => this.radarManager.ColorHostile,
                SensorTarget.TargetAllegiance.JumpGate => this.radarManager.JumpGateColor,
                _ => throw new Exception("Unexpected Sensor Target Allegiance")
            };
        }
    }
}

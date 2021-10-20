using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Ship.Sensors
{
    public class RadarManager : MonoBehaviour
    {

        [SerializeField] private Transform ownPosition;
        [SerializeField] private float radarRadius = 0.3f;
        [SerializeField] private AnimationCurve realDistanceToSensorDistance;
        [SerializeField] private GameObject sensorObjectPrefab;

        [SerializeField] private Sprite shipTargetSprite;
        [SerializeField] private Sprite stationTargetSprite;
        [SerializeField] private Sprite missileTargetSprite;

        [SerializeField] private Color friendlyColor;
        [SerializeField] private Color neutralColor;
        [SerializeField] private Color hostileColor;

        public Color ColorFriendly => this.friendlyColor;
        public Color ColorNeutral => this.neutralColor;
        public Color ColorHostile => this.hostileColor;

        public Sprite SpriteShip => this.shipTargetSprite;
        public Sprite SpriteStation => this.stationTargetSprite;
        public Sprite SpriteMissile => this.missileTargetSprite;

        public Transform OwnPosition => this.ownPosition;


        private void Start()
        {
            foreach (var entry in GameManager.Instance.EnemyManager.Enemies)
            {
                this.HandleNewSensorTargetCreated(entry);
            }

            GameManager.Instance.EnemyManager.NewEnemySpawnedEvent += enemy =>
                this.HandleNewSensorTargetCreated(enemy.GetComponent<SensorTarget>());
        }


        public Vector3 ApplyPositionTransformation(Vector3 targetPosition)
        {
            var relativeToRadar = targetPosition - this.ownPosition.position;
            var rotationToApply = this.transform.worldToLocalMatrix.rotation;
            var withRotation = rotationToApply * relativeToRadar;
            return withRotation.normalized * (this.realDistanceToSensorDistance.Evaluate(relativeToRadar.magnitude) * this.radarRadius);
        }

        private void HandleNewSensorTargetCreated(SensorTarget target)
        {
            var sensorObjectGO = Instantiate(this.sensorObjectPrefab, this.transform);
            sensorObjectGO.layer = 10;
            sensorObjectGO.GetComponent<SensorObject>().Init(target, this);

        }

    }
}

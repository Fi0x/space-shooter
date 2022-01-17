using System.Collections.Generic;
using Ship.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;

        [SerializeField] [ReadOnlyInspector] private List<SensorTarget> enemies = new List<SensorTarget>();


        public IReadOnlyList<SensorTarget> Enemies => this.enemies;

        public void NotifyAboutNewEnemySpawned(GameObject enemy)
        {
            var sensorTarget = enemy.GetComponent<SensorTarget>();
            sensorTarget.TargetDestroyedEvent += target => this.enemies.Remove(target);
            sensorTarget.Init(SensorTarget.TargetType.Ship, SensorTarget.TargetAllegiance.Hostile);
            this.enemies.Add(sensorTarget);
            RadarManager.InvokeRadarObjectSpawnedEvent(enemy);
        }

        public void RemoveAllEnemies()
        {
            foreach (var enemy in this.Enemies)
            {
                Destroy(enemy.gameObject);
            }
        }
    }

}
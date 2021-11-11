using System;
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

        public IReadOnlyList<SensorTarget> Enemies => enemies;

        public delegate void NewEnemySpawnedDelegate(GameObject enemy);

        public event NewEnemySpawnedDelegate NewEnemySpawnedEvent;

        public void SpawnNewEnemy(Vector3 position)
        {
            var enemy = Instantiate(this.enemyPrefab, position, Random.rotation, this.transform);
            var sensorTarget = enemy.GetComponent<SensorTarget>();
            sensorTarget.TargetDestroyedEvent += target => enemies.Remove(target);
            sensorTarget.Init(SensorTarget.TargetType.Ship, SensorTarget.TargetAllegiance.Hostile);
            enemies.Add(sensorTarget);
            NewEnemySpawnedEvent?.Invoke(enemy);
        }

        public void RemoveAllEnemies()
        {
            foreach (var enemy in Enemies)
            {
                Destroy(enemy.gameObject);
            }
        }
    }
}
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

        public void RemoveAllEnemies()
        {
            foreach (var enemy in this.Enemies)
            {
                Destroy(enemy.gameObject);
            }
        }
    }

}
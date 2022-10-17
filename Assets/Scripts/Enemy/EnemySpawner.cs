using UnityEngine;
using Manager;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Basic Enemy")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int minBasic = 5;
        [SerializeField] private int maxBasic = 10;
        
        [Header("Elite Enemy")]
        [SerializeField] private GameObject eliteEnemyPrefab;
        [SerializeField] private int minElite = 0;
        [SerializeField] private int maxElite = 1;

        [Header("Spawn Ranges")]
        [SerializeField] private float minSpawnRange = 300;
        [SerializeField] private float maxSpawnRange = 400;

        public void SetDifficulty(float difficulty)
        {
            minBasic = (int) (difficulty * 1f);
            maxBasic = (int) (difficulty * 1f) + 1;
            minElite = (int) (difficulty * 1f);
            maxElite = (int) (difficulty * 1f) + 1;
        }
        
        public void SpawnBasic()
        {
            var flockCount = Random.Range(minBasic, maxBasic);
            for(var i = 0; i < flockCount; i++)
            {
                var randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)).normalized;
                var spawnRange = Random.Range(this.minSpawnRange, this.maxSpawnRange);
                var spawnPosition = randomDirection * spawnRange;
                if(GameManager.Instance.Player != null) spawnPosition = GameManager.Instance.Player.transform.position + randomDirection * spawnRange;
                
                Instantiate(this.enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
        
        public void SpawnElite()
        {
            var flockCount = Random.Range(minElite, maxElite);
            for(var i = 0; i < flockCount; i++)
            {
                var randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)).normalized;
                var spawnRange = Random.Range(this.minSpawnRange, this.maxSpawnRange);
                var spawnPosition = randomDirection * spawnRange;
                if(GameManager.Instance.Player != null) spawnPosition = GameManager.Instance.Player.transform.position + randomDirection * spawnRange;
                
                Instantiate(eliteEnemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
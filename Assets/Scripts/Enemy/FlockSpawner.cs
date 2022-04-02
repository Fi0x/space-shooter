using UnityEngine;
using Manager;

namespace Enemy
{
    public class FlockSpawner : MonoBehaviour
    {
        [Header("Flock Prefab")]
        [SerializeField] private GameObject flockPrefab;

        [Header("Flock Count")]
        [SerializeField] private int minFlocks = 1;
        [SerializeField] private int maxFlocks = 5;

        [Header("Spawn Ranges")]
        [SerializeField] private float minSpawnRange = 300;
        [SerializeField] private float maxSpawnRange = 400;

        public void SetDifficulty(float difficulty)
        {
            minFlocks = (int) (difficulty * 1f);
            maxFlocks = (int) (difficulty * 1f) + 1;
            
        }
        
        public void SpawnFlocks()
        {
            var flockCount = Random.Range(this.minFlocks, this.maxFlocks);
            for(var i = 0; i < flockCount; i++)
            {
                var randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)).normalized;
                var spawnRange = Random.Range(this.minSpawnRange, this.maxSpawnRange);
                var spawnPosition = randomDirection * spawnRange;
                if(GameManager.Instance.Player != null) spawnPosition = GameManager.Instance.Player.transform.position + randomDirection * spawnRange;
                
                Instantiate(this.flockPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
using System;
using UnityEngine;
using Manager;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Basic Enemy")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int minBasic = 3;
        [SerializeField] private int maxBasic = 5;
        
        [Header("Elite Enemy")]
        [SerializeField] private GameObject eliteEnemyPrefab;
        [SerializeField] private int minElite = 0;
        [SerializeField] private int maxElite = 0;

        [Header("Spawn Ranges")]
        [SerializeField] private float minSpawnRange = 300;
        [SerializeField] private float maxSpawnRange = 400;
        [SerializeField] private LayerMask mask;

        private float difficulty = 1;

        public void SetDifficulty(float newDifficulty)
        {
            difficulty = newDifficulty;
        }

        private void GetDifficulty()
        {
            difficulty = GameManager.Instance.difficulty;
        }
        
        public void SpawnBasic()
        {
            GetDifficulty();
            var flockCount = Random.Range(minBasic, maxBasic);
            Debug.Log("Enemies Basic:" + (int)(flockCount + 1.2f * difficulty));
            int enemies = Math.Max((int)(flockCount + 1.0f * difficulty), 3);
            //Debug.Log("Number enemies: " + enemies);
            for(int i = 0; i < enemies; i++)
            {
                var randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)).normalized;
                var spawnRange = Random.Range(this.minSpawnRange, this.maxSpawnRange);
                var spawnPosition = randomDirection * spawnRange;
                if(GameManager.Instance.Player != null) spawnPosition = GameManager.Instance.Player.transform.position + randomDirection * spawnRange;
                var enemy = Instantiate(this.enemyPrefab, spawnPosition, Quaternion.identity);
                
                Collider[] collisions = new Collider[20];
                // Physics.OverlapSphereNonAlloc(enemy.transform.position, 20f, collisions, mask);
                // foreach (var c in collisions)
                // {
                //     if(c == null) return;
                //     //Debug.Log("Deleted" + c.gameObject.name);
                //     Destroy(c.gameObject);
                // }
                // collisions = new Collider[20];
            }
        }
        
        public void SpawnElite()
        {
            GetDifficulty();
            var flockCount = Random.Range(minElite, maxElite);
            int enemies = (int) (flockCount + 0.35 * difficulty - 1);
            Debug.Log("Elite Enemies:" + enemies);
            for(int i = 0; i < enemies; i++)
            {
                var randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)).normalized;
                var spawnRange = Random.Range(this.minSpawnRange, this.maxSpawnRange);
                var spawnPosition = randomDirection * spawnRange;
                if(GameManager.Instance.Player != null) spawnPosition = GameManager.Instance.Player.transform.position + randomDirection * spawnRange;
                
                Collider[] collisions = new Collider[20];
                var enemy = Instantiate(eliteEnemyPrefab, spawnPosition, Quaternion.identity);
                Physics.OverlapSphereNonAlloc(enemy.transform.position, 40f, collisions, mask);
                foreach (var c in collisions)
                {
                    if(c == null) return;
                    //Debug.Log("Deleted" + c.gameObject.name);
                    Destroy(c.gameObject);
                }
            }
        }
    }
}
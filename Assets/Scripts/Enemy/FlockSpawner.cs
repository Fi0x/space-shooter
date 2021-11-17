using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class FlockSpawner : MonoBehaviour
{
    [Header("Flock Prefab")]
    [SerializeField] private GameObject flockPrefab;

    [Header("Flock Count")]
    [SerializeField] private int minFlocks;
    [SerializeField] private int maxFlocks;

    [Header("Spawn Ranges")]
    [SerializeField] private float minSpawnRange;
    [SerializeField] private float maxSpawnRange;

    // Start is called before the first frame update
    void Start()
    {
        int flockCount = Random.Range(minFlocks, maxFlocks);

        for(int i = 0; i < flockCount; i++)
        {
            Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f));

            float spawnRange = Random.Range(minSpawnRange, maxSpawnRange);


            Vector3 spawnPosition = GameManager.Instance.Player.transform.position + randomDirection * spawnRange;

            Instantiate(flockPrefab, spawnPosition, Quaternion.identity);
        }
    }

    
}

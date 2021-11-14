using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class BoidController : MonoBehaviour
{

    [SerializeField]
    public List<Boid>[] boids;

    [SerializeField]
    private int SwarmCount;

    [SerializeField]
    private GameObject EnemyPrefab;

    [SerializeField]
    private int minEnemyCount;
    [SerializeField]
    private int maxEnemyCount;

    [SerializeField]
    private EnemyManager enemyManager;

    
    // Called on LevelSetup, instantiates Swarms of Enemies
    public void InitializeBoids(int SwarmCount_, Vector3[] spawnPositions)
    {
        SwarmCount = SwarmCount_;
        boids = new List<Boid>[SwarmCount];

        for(int i = 0; i < SwarmCount; i++)
        {
            boids[i] = new List<Boid>();
        }

        Debug.Log("InitializeBoids, spawnPositions.Length: " + spawnPositions.Length);
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            int enemyCount = UnityEngine.Random.Range(minEnemyCount, maxEnemyCount);


            GameObject swarm = new GameObject("Swarm" + i);

            for (int j = 0; j < enemyCount; j++) 
            {
                GameObject boid = Instantiate(EnemyPrefab, spawnPositions[i] + Random.onUnitSphere * 10, 
                    Quaternion.identity);

                EnemyAI enemyAIScript = boid.GetComponent<EnemyAI>();
                enemyAIScript.InitiliazeEnemyAI(this);

                Boid boidScript = boid.GetComponent<Boid>();
                boidScript.SwarmIndex = i;
                

                enemyManager.NotifyAboutNewEnemySpawned(boid);

                // Add to list
                boids[i].Add(boidScript);
            }
        }
    }


    // returns the other boids in the swarm to simulate the movement
    public List<Boid> GetOtherBoids(int SwarmIndex)
    {
        Debug.Log(boids[SwarmIndex].Count);
        return boids[SwarmIndex];
    }

    // removes boid from List/Swarm on death
    public void RemoveBoidOnDeath(int SwarmIndex, Boid boid)
    {
        boids[SwarmIndex].Remove(boid);
    }


    // Sets a new roaming position for the Swarm, once one of them reaches it
    public void SetNewRoamingPosition(int SwarmIndex, Vector3 newRoamingPos)
    {
        foreach(Boid boid in boids[SwarmIndex])
        {
            boid.GetComponent<EnemyAI>().roamingPosition = newRoamingPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{

    [SerializeField]
    public List<Boid>[] boids;

    [SerializeField]
    private int SwarmCount;

    [SerializeField]
    private GameObject EnemyPrefab;

    
    // Called on LevelSetup, instantiates Swarms of Enemies
    public void InitializeBoids(int SwarmCount_, Vector3[] spawnPositions)
    {
        SwarmCount = SwarmCount_;
        boids = new List<Boid>[SwarmCount];

        for(int i = 0; i < SwarmCount; i++)
        {
            boids[i] = new List<Boid>();
        }

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            int enemyCount = UnityEngine.Random.Range(4, 9);


            GameObject swarm = new GameObject("Swarm" + i);

            for (int j = 0; j < enemyCount; j++) 
            {
                Instantiate(EnemyPrefab, spawnPositions[i] + Random.onUnitSphere * 10, Quaternion.identity, swarm.transform);
            }
        }
    }


    // returns the other boids in the swarm to simulate the movement
    public List<Boid> GetOtherBoids(int SwarmIndex)
    {
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

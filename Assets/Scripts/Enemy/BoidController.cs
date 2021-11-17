using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class BoidController : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private Boid flockUnitPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;

    [Header("Speed Setup")]
    [Range(0, 100)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 100)]
    [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }

    [Header("Detection Distances")]
    [Range(0, 100)]
    [SerializeField] private float _cohesionDistance;
    public float cohesionDistance { get { return _cohesionDistance; } }
    [Range(0, 100)]
    [SerializeField] private float _avoidanceDistance;
    public float avoidanceDistance { get { return _avoidanceDistance; } }
    [Range(0, 100)]
    [SerializeField] private float _alignmentDistance;
    public float alignmentDistance { get { return _alignmentDistance; } }
    [Range(0, 20)]
    [SerializeField] private float _obstacleDistance;
    public float obstacleDistance { get { return _obstacleDistance; } }

    [Header("Behavior Weights")]
    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }
    [Range(0, 10)]
    [SerializeField] private float _avoidanceWeight;
    public float avoidanceWeight { get { return _avoidanceWeight; } }
    [Range(0, 10)]
    [SerializeField] private float _alignmentWeight;
    public float alignmentWeight { get { return _alignmentWeight; } }
    [Range(0, 100)]
    [SerializeField] private float _obstacleWeight;
    public float obstacleWeight { get { return _obstacleWeight; } }


    [Header("Roaming Position")]
    [SerializeField] private Vector3 _roamingPosition;
    public Vector3 roamingPosition { get { return _roamingPosition; } }
    [SerializeField] private float roamingPosReachedDistance;
    [SerializeField] private float minDistanceFromPlayer;
    [SerializeField] private float maxDistanceFromPlayer;

    //public Boid[] allUnits { get; set; }
    public List<Boid> allUnits { get; set; }

    private void Start()
    {
        GenerateUnits();
        GetRoamingPosition();
    }

    /*
    private void Update()
    {
        for (int i = 0; i < allUnits.Length; i++)
        {
            if (Vector3.Distance(allUnits[i].myTransform.position, roamingPosition) <= roamingPosReachedDistance)
            {
                GetRoamingPosition();
            }

            //allUnits[i].MoveUnit();
        }
    }
    */

    /*
    private void GenerateUnits()
    {
        allUnits = new Boid[flockSize];

        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            allUnits[i].AssignFlock(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
            allUnits[i].BoidHelper();
        }
    }*/
    
    private void GenerateUnits()
    {
        allUnits = new List<Boid>();

        for(int i = 0; i < flockSize; i++)
        {
            /*
            var randomVector = transform.position + Random.onUnitSphere * 10;
            var rotation = Quaternion.Euler(0, 0, 0);
            allUnits[i] = Instantiate(flockUnitPrefab, randomVector, rotation);
            allUnits[i].AssignFlock(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
            allUnits[i].BoidHelper();
            allUnits[i].GetComponent<EnemyAI>().InitiliazeEnemyAI(this);

            // parent
            allUnits[i].transform.SetParent(this.transform);
            */

            var randomVector = transform.position + Random.onUnitSphere * 10;
            var rotation = Quaternion.Euler(0, 0, 0);
            Boid boid = Instantiate(flockUnitPrefab, randomVector, rotation);
            boid.AssignFlock(this);
            boid.InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
            boid.BoidHelper();
            boid.GetComponent<EnemyAI>().InitiliazeEnemyAI(this);

            // parent
            boid.transform.SetParent(this.transform);
            // Add to List
            allUnits.Add(boid);
        }
    }

    private void GetRoamingPosition()
    {
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f));

        Vector3 newRoamingPosition = GameManager.Instance.Player.transform.position + randomDirection 
            * UnityEngine.Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);

        _roamingPosition = newRoamingPosition;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = newRoamingPosition;
        cube.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void SetNewRoamingPosition()
    {
        GetRoamingPosition();
    }



    // removes Boid from allUnits
    public void RemoveBoid(Boid boid)
    {
        allUnits.Remove(boid);
    }
}

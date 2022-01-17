using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Manager;

namespace Enemy
{
    public class BoidController : MonoBehaviour
    {
        [Header("Spawn Setup")]
        [SerializeField] private Boid flockUnitPrefab;
        [SerializeField] private int flockSize;

        [Header("Speed Setup")]
        [Range(0, 100)]
        [SerializeField] private float minSpeed;
        public float MinSpeed => this.minSpeed;

        [Range(0, 100)]
        [SerializeField] private float maxSpeed;
        public float MaxSpeed => this.maxSpeed;

        [Header("Detection Distances")]
        [Range(0, 100)]
        [SerializeField] private float cohesionDistance;
        public float CohesionDistance => this.cohesionDistance;

        [Range(0, 100)]
        [SerializeField] private float avoidanceDistance;
        public float AvoidanceDistance => this.avoidanceDistance;

        [Range(0, 100)]
        [SerializeField] private float alignmentDistance;
        public float AlignmentDistance => this.alignmentDistance;

        [Range(0, 20)]
        [SerializeField] private float obstacleDistance;
        public float ObstacleDistance => this.obstacleDistance;

        [Header("Behavior Weights")]
        [Range(0, 10)]
        [SerializeField] private float cohesionWeight;
        public float CohesionWeight => this.cohesionWeight;

        [Range(0, 10)]
        [SerializeField] private float avoidanceWeight;
        public float AvoidanceWeight => this.avoidanceWeight;

        [Range(0, 10)]
        [SerializeField] private float alignmentWeight;
        public float AlignmentWeight => this.alignmentWeight;

        [Range(0, 100)]
        [SerializeField] private float obstacleWeight;
        public float ObstacleWeight => this.obstacleWeight;


        [Header("Roaming Position")]
        [SerializeField] private Vector3 roamingPosition;
        public Vector3 RoamingPosition => this.roamingPosition;
        [SerializeField] private float roamingPosReachedDistance;
        [SerializeField] private float minDistanceFromPlayer;
        [SerializeField] private float maxDistanceFromPlayer;

        public List<Boid> AllUnits { get; private set; }

        private void Start()
        {
            this.GenerateUnits();
            this.GetRoamingPosition();
        }

        private void GenerateUnits()
        {
            this.AllUnits = new List<Boid>();

            for(var i = 0; i < this.flockSize; i++)
            {
                var randomVector = this.transform.position + Random.onUnitSphere * 10;
                var rotation = Quaternion.Euler(0, 0, 0);
                var boid = Instantiate(this.flockUnitPrefab, randomVector, rotation);
                boid.AssignFlock(this);
                boid.InitializeSpeed(Random.Range(this.MinSpeed, this.MaxSpeed));
                boid.BoidHelper();
                boid.GetComponent<EnemyAI>().InitializeEnemyAI(this);

                // parent
                boid.transform.SetParent(this.transform);
                // Add to List
                this.AllUnits.Add(boid);

                // notify about sensor target
                GameManager.Instance.EnemyManager.NotifyAboutNewEnemySpawned(boid.gameObject);
            }
        }

        private void GetRoamingPosition()
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f));

            Vector3 newRoamingPosition = GameManager.Instance.Player.transform.position + randomDirection 
                * Random.Range(this.minDistanceFromPlayer, this.maxDistanceFromPlayer);

            this.roamingPosition = newRoamingPosition;
        }

        public void SetNewRoamingPosition()
        {
            this.GetRoamingPosition();
        }
    
        // removes Boid from allUnits
        public void RemoveBoid(Boid boid)
        {
            this.AllUnits.Remove(boid);
        }
    }
}
using Ship;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Enemy
{
    public class Boid : MonoBehaviour
    {
        [SerializeField] private float FOVAngle;
        [SerializeField] private float smoothDamp;

        private List<Boid> cohesionNeighbours = new List<Boid>();
        private List<Boid> avoidanceNeighbours = new List<Boid>();
        private List<Boid> alignmentNeighbours = new List<Boid>();
        private BoidController assignedFlock;
        private Vector3 currentVelocity;
        private float speed;
        private Vector3 currentObstacleAvoidanceVector;

        public Transform myTransform { get; set; }

        private void Awake()
        {
            this.myTransform = this.transform;
        }


        public void AssignFlock(BoidController flock)
        {
            this.assignedFlock = flock;
        }

        public void InitializeSpeed(float speed)
        {
            this.speed = speed;
        }

        public void MoveUnit()
        {
            this.FindNeighbours();
            this.CalculateSpeed();
            var cohesionVector = this.CalculateCohesionVector() * this.assignedFlock.CohesionWeight;
            var avoidanceVector = this.CalculateAvoidanceVector() * this.assignedFlock.AvoidanceWeight;
            var alignmentVector = this.CalculateAlignmentVector() * this.assignedFlock.AlignmentWeight;

            var moveVector = cohesionVector + avoidanceVector + alignmentVector;
            var obstacleVector = Vector3.zero;

            if (this.IsHeadingForCollision(moveVector))
            {
                obstacleVector = this.FindUnobstructedDirection();
                obstacleVector = obstacleVector.normalized;
            }
            else
            {
                var flockCenterVector = Vector3.zero;
                for (var i = 0; i < this.assignedFlock.AllUnits.Count; i++)
                    flockCenterVector += this.assignedFlock.AllUnits[i].transform.position;

                flockCenterVector /= this.assignedFlock.AllUnits.Count;

                if (Vector3.Distance(flockCenterVector, this.myTransform.position) > 50)
                {
                    var flockCenterDirection = flockCenterVector - this.myTransform.position;
                    this.speed *= 1.5f;
                    moveVector = flockCenterDirection.normalized * this.speed;
                    this.myTransform.forward = moveVector;
                    this.myTransform.position += moveVector * Time.deltaTime;
                    return;
                }
            }

            moveVector += obstacleVector * this.assignedFlock.ObstacleWeight;

            moveVector = Vector3.SmoothDamp(this.myTransform.forward, moveVector, ref this.currentVelocity, this.smoothDamp);
            moveVector = moveVector.normalized * this.speed;
            if (moveVector == Vector3.zero)
                moveVector = this.transform.forward;


            this.myTransform.forward = moveVector;
            this.myTransform.position += moveVector * Time.deltaTime;
        }

        private void CalculateSpeed()
        {
            //speed = ShipMovementHandler.TotalMaxSpeed / 10;
            //this.speed = 20.0f;
        }

        private void FindNeighbours()
        {
            this.cohesionNeighbours.Clear();
            this.avoidanceNeighbours.Clear();
            this.alignmentNeighbours.Clear();
            var allUnits = this.assignedFlock.AllUnits;
            for (int i = 0; i < allUnits.Count; i++)
            {
                var currentUnit = allUnits[i];
                if (currentUnit != this)
                {
                    float currentNeighbourDistanceSqr =
                        Vector3.SqrMagnitude(currentUnit.transform.position - this.myTransform.position);
                    if (currentNeighbourDistanceSqr <= this.assignedFlock.CohesionDistance * this.assignedFlock.CohesionDistance) this.cohesionNeighbours.Add(currentUnit);

                    if (currentNeighbourDistanceSqr <= this.assignedFlock.AvoidanceDistance * this.assignedFlock.AvoidanceDistance) this.avoidanceNeighbours.Add(currentUnit);

                    if (currentNeighbourDistanceSqr <= this.assignedFlock.AlignmentDistance * this.assignedFlock.AlignmentDistance) this.alignmentNeighbours.Add(currentUnit);
                }
            }
        }

        private Vector3 CalculateCohesionVector()
        {
            var cohesionVector = Vector3.zero;
            if (this.cohesionNeighbours.Count == 0)
                return cohesionVector;

            int neighboursInFOV = 0;
            for (int i = 0; i < this.cohesionNeighbours.Count; i++)
            {
                if (this.IsInFOV(this.cohesionNeighbours[i].myTransform.position))
                {
                    neighboursInFOV++;
                    cohesionVector += this.cohesionNeighbours[i].myTransform.position;
                }
            }

            cohesionVector /= neighboursInFOV;
            cohesionVector -= this.myTransform.position;
            cohesionVector = cohesionVector.normalized;
            return cohesionVector;
        }

        private Vector3 CalculateAlignmentVector()
        {
            var alignementVector = (this.assignedFlock.RoamingPosition - this.myTransform.position).normalized;
            if (this.alignmentNeighbours.Count == 0)
                return alignementVector;

            int neighboursInFOV = 0;
            for (int i = 0; i < this.alignmentNeighbours.Count; i++)
            {
                if (this.IsInFOV(this.alignmentNeighbours[i].myTransform.position))
                {
                    neighboursInFOV++;
                    alignementVector += this.alignmentNeighbours[i].myTransform.forward;
                }
            }

            alignementVector /= neighboursInFOV;
            alignementVector = alignementVector.normalized;
            return alignementVector;
        }

        private Vector3 CalculateAvoidanceVector()
        {
            var avoidanceVector = Vector3.zero;
            if (this.avoidanceNeighbours.Count == 0)
                return avoidanceVector;

            int neighboursInFOV = 0;
            for (int i = 0; i < this.avoidanceNeighbours.Count; i++)
            {
                if (this.IsInFOV(this.avoidanceNeighbours[i].myTransform.position))
                {
                    neighboursInFOV++;
                    avoidanceVector += (this.myTransform.position - this.avoidanceNeighbours[i].myTransform.position);
                }
            }

            avoidanceVector /= neighboursInFOV;
            avoidanceVector = avoidanceVector.normalized;
            return avoidanceVector;
        }

        private bool IsHeadingForCollision(Vector3 direction)
        {
            RaycastHit hit;
            return Physics.SphereCast(this.myTransform.position, 1.0f, direction, out hit, this.assignedFlock.ObstacleDistance,
                LayerMask.GetMask("Scenery"));
        }

        private Vector3 FindUnobstructedDirection()
        {
            float furthestUnobstructedDst = 0;

            float radius = 1.0f;
            float ViewRadius = this.assignedFlock.ObstacleDistance;
            LayerMask layerMask = LayerMask.GetMask("Scenery");
            for (int i = 0; i < this.directions.Length; i++)
            {
                Vector3 dir = this.transform.TransformDirection(this.directions[i]);
                Ray ray = new Ray(this.transform.position, dir);
                Debug.DrawRay(this.transform.position, dir * this.assignedFlock.ObstacleDistance, Color.cyan, 0.2f);
                if (!Physics.SphereCast(ray, radius, ViewRadius, layerMask))
                {
                    return dir;
                }
            }

            return Vector3.zero;
        }

        private bool IsInFOV(Vector3 position)
        {
            return Vector3.Angle(this.myTransform.forward, position - this.myTransform.position) <= this.FOVAngle;
        }

        // obstacle avoidance
        public const int numViewDirections = 300;
        public Vector3[] directions;
        private readonly float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;

        public void BoidHelper()
        {
            this.directions = new Vector3[numViewDirections];

            float angleIncrement = Mathf.PI * 2 * this.goldenRatio;
            for (int i = 0; i < numViewDirections; i++)
            {
                float t = (float)i / numViewDirections;
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = angleIncrement * i;

                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);
                this.directions[i] = new Vector3(x, y, z);
            }
        }

        public void RemoveBoidFromAssignedFlock()
        {
            this.assignedFlock.RemoveBoid(this);
        }
    }
}
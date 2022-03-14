using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{

    public class Boid : MonoBehaviour
    {
        [FormerlySerializedAs("FOVAngle")] [SerializeField] private float fovAngle;
        [SerializeField] private float smoothDamp;
        [SerializeField] private float desiredSpeed = 20f;
        
        internal List<Boid> CohesionNeighbours { get; } = new List<Boid>();
        internal  List<Boid> AvoidanceNeighbours { get; } =  new List<Boid>();
        internal  List<Boid> AlignmentNeighbours { get; } =  new List<Boid>();
        private BoidController assignedFlock;

        public BoidController ParentFlock => this.assignedFlock;
        public float FovAngle => this.fovAngle;

        private BoidObstacleAvoidanceHelper boidObstacleAvoidanceHelper = new BoidObstacleAvoidanceHelper(300);
        
        public float DesiredSpeed => this.desiredSpeed;
        

        public void AssignFlock(BoidController flock)
        {
            assignedFlock = flock;
        }

        public Vector3 GetDesiredDirectionAndVelocity()
        {
            BoidCalculationsHelper.RebuildNeighbourLists(this);
            var moveVector = BoidCalculationsHelper.CalculateMoveVectorWeightForces(this);
            

            if (IsHeadingForCollision(moveVector))
            {
                //Debug.Log("IsHeadingForCollision");
                var obstacleVector = FindUnobstructedDirection().normalized;
                //Debug.Log("obstacleVector: " + obstacleVector);
                //Debug.Log("transform.forward: " + this.transform.forward);
                moveVector += obstacleVector * this.ParentFlock.obstacleWeight;
            }
            else
            {
                // There is no collision about to happen
                var flockCenterVector = this.ParentFlock.CalculateFlockCenter();

                // If this Boid is too far away from the centre, nudge it towards it
                if (Vector3.Distance(flockCenterVector, this.transform.position) > 50)
                {
                    var ownTransform = this.transform;
                    var flockCenterDirection = flockCenterVector - ownTransform.position;
                    moveVector += flockCenterDirection.normalized;
                }
            }
            return moveVector.normalized * this.DesiredSpeed;
        }
        
        private bool IsHeadingForCollision(Vector3 direction)
        {
            //Debug.DrawRay(transform.position, direction * assignedFlock.obstacleDistance, Color.green, 0.2f);
            if (Physics.SphereCast(transform.position, 1.0f, direction, out _, assignedFlock.obstacleDistance, LayerMask.GetMask("Scenery")))
            {

                return true;
            }

            return false;

        }

        private Vector3 FindUnobstructedDirection()
        {
            const float radius = 1.0f;
            var viewRadius = assignedFlock.obstacleDistance;
            LayerMask sceneryLayerMask = LayerMask.GetMask("Scenery");
            foreach (var direction in this.boidObstacleAvoidanceHelper.Directions)
            {
                var ownTransform = this.transform;
                var dir = ownTransform.TransformDirection(direction);
                var ray = new Ray(ownTransform.position, dir);
                Debug.DrawRay(ownTransform.position, dir * assignedFlock.obstacleDistance, Color.cyan);
                if (!Physics.SphereCast(ray, radius, viewRadius, sceneryLayerMask))
                {
                    return dir;
                }
            }

            return Vector3.zero;
        }
        

        // remove Boid from assignedFlock
        public void RemoveBoidFromAssignedFlock()
        {
            assignedFlock.RemoveBoid(this);
        }
    }

}
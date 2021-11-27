using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        myTransform = transform;
    }


    public void AssignFlock(BoidController flock)
    {
        assignedFlock = flock;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }




    public void MoveUnit()
    {
        FindNeighbours();
        CalculateSpeed();
        Vector3 cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
        Vector3 avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
        Vector3 alignmentVector = CalculateAlignmentVector() * assignedFlock.alignmentWeight;

        /* float targetWeight = 0.2f;
         Vector3 targetPoint = new Vector3(50, 50, 50);
         Vector3 directionToTargetWeighted = (targetPoint - myTransform.position).normalized * targetWeight;
        */
        var moveVector = cohesionVector + avoidanceVector + alignmentVector;

        Vector3 obstacleVector = Vector3.zero;

        if (IsHeadingForCollision(moveVector))
        {
            //Debug.Log("IsHeadingForCollision");
            obstacleVector = FindUnobstructedDirection();
            //Debug.Log("obstacleVector: " + obstacleVector);
            //Debug.Log("transform.forward: " + myTransform.forward);
            obstacleVector = obstacleVector.normalized;
        } else
        {
            
            Vector3 flockCenterVector = Vector3.zero;
            for(int i = 0; i < assignedFlock.allUnits.Count; i++)
            {
                flockCenterVector += assignedFlock.allUnits[i].transform.position;
            }
            flockCenterVector /= assignedFlock.allUnits.Count;

            if (Vector3.Distance(flockCenterVector, myTransform.position) > 50)
            {
                Vector3 flockCenterDirection = flockCenterVector - myTransform.position;
                speed *= 1.5f;
                moveVector = flockCenterDirection.normalized * speed;
                myTransform.forward = moveVector;
                myTransform.position += moveVector * Time.deltaTime;
                return;
            }
        }

        moveVector += obstacleVector * assignedFlock.obstacleWeight;


        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
        {
            moveVector = transform.forward;
        }

        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;

        //Debug.DrawRay(myTransform.position, moveVector * 2, Color.red, 0.25f);
    }

    private void CalculateSpeed()
    {
        speed = ShipMovementHandler.TotalMaxSpeed / 10;
        speed = 20.0f;
        return;
        if (cohesionNeighbours.Count == 0)
        {
            return;
        }

        speed = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }
        speed /= cohesionNeighbours.Count;
        Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
    }

    private void FindNeighbours()
    {
        cohesionNeighbours.Clear();
        avoidanceNeighbours.Clear();
        alignmentNeighbours.Clear();
        var allUnits = assignedFlock.allUnits;
        for (int i = 0; i < allUnits.Count; i++)
        {
            var currentUnit = allUnits[i];
            if (currentUnit != this)
            {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.transform.position - myTransform.position);
                if (currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
                {
                    avoidanceNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.alignmentDistance * assignedFlock.alignmentDistance)
                {
                    alignmentNeighbours.Add(currentUnit);
                }
            }
        }
    }

    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;
        if (cohesionNeighbours.Count == 0)
        {
            return cohesionVector;
        }

        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }

        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;
        return cohesionVector;
    }

    private Vector3 CalculateAlignmentVector()
    {
        var alignementVector = (assignedFlock.roamingPosition - myTransform.position).normalized;
        if (alignmentNeighbours.Count == 0)
        {
            return alignementVector;
        }

        int neighboursInFOV = 0;
        for (int i = 0; i < alignmentNeighbours.Count; i++)
        {
            if (IsInFOV(alignmentNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                alignementVector += alignmentNeighbours[i].myTransform.forward;
            }
        }

        alignementVector /= neighboursInFOV;
        alignementVector = alignementVector.normalized;
        return alignementVector;
    }

    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;
        if (avoidanceNeighbours.Count == 0)
        {
            return avoidanceVector;
        }

        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbours.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbours[i].myTransform.position);
            }
        }

        avoidanceVector /= neighboursInFOV;
        avoidanceVector = avoidanceVector.normalized;
        return avoidanceVector;
    }

    public bool IsHeadingForCollision(Vector3 direction)
    {
        //Debug.DrawRay(transform.position, direction * assignedFlock.obstacleDistance, Color.green, 0.2f);
        RaycastHit hit;
        if (Physics.SphereCast(myTransform.position, 1.0f, direction, out hit, assignedFlock.obstacleDistance, LayerMask.GetMask("Scenery")))
        {

            return true;
        }

        return false;

    }

    public Vector3 FindUnobstructedDirection()
    {
        float furthestUnobstructedDst = 0;
        RaycastHit hit;


        float radius = 1.0f;
        float ViewRadius = assignedFlock.obstacleDistance;
        LayerMask layerMask = LayerMask.GetMask("Scenery");
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(directions[i]);
            Ray ray = new Ray(transform.position, dir);
            Debug.DrawRay(transform.position, dir * assignedFlock.obstacleDistance, Color.cyan, 0.2f);
            if (!Physics.SphereCast(ray, radius, ViewRadius, layerMask))
            {
                return dir;
            }
        }

        return Vector3.zero;
    }



    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }


    // obstacle avoidance
    public const int numViewDirections = 300;
    public Vector3[] directions;
    float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;

    public void BoidHelper()
    {
        directions = new Vector3[numViewDirections];

        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numViewDirections; i++)
        {
            float t = (float)i / numViewDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            directions[i] = new Vector3(x, y, z);
            //Debug.DrawRay(transform.position, directions[i] *  10, Color.red);
        }
    }



    // remove Boid from assignedFlock
    public void RemoveBoidFromAssignedFlock()
    {
        assignedFlock.RemoveBoid(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    // obstacle avoidance
    public const int numViewDirections = 300;
    public Vector3[] directions;
    float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;

    // boid settings
    [SerializeField] public int SwarmIndex;
    public float NoClumpingRadius { get; set; }
    public float LocalAreaRadius { get; set; }
    public float Speed { get; set; }
    public float SteeringSpeed { get; set; }

    // boid weights 
    public float seperationWeight = 0.5f;
    public float alignmentWeight = 0.34f;
    public float cohesionWeight = 0.16f;

    float ColliderRadius;


    // Start is called before the first frame update
    void Start()
    {
        BoidHelper();

        Speed = 10f;
        SteeringSpeed = 100f;
        NoClumpingRadius = 17.5f;
        LocalAreaRadius = 50f;

        ColliderRadius = 12.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BoidHelper()
    {
        directions = new Vector3[numViewDirections];

        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for(int i = 0; i < numViewDirections; i++)
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

    public Vector3 FindUnobstructedDirection(Vector3 bestDir)
    {
        float furthestUnobstructedDst = 0;
        RaycastHit hit;

        
        float radius = ColliderRadius;
        float ViewRadius = 17.25f;
        LayerMask layerMask = LayerMask.GetMask("Scenery");
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(directions[i]);
            Ray ray = new Ray(transform.position, dir);

            if(!Physics.SphereCast(ray, radius, ViewRadius, layerMask))
            {
                return dir;
            }
        }
        
        Debug.Log(bestDir);
        return bestDir;
    }

    public bool IsHeadingForCollision(Vector3 direction)
    {
        //Debug.DrawRay(transform.position, transform.forward * 5, Color.green, 0.2f);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, ColliderRadius, direction, out hit, 17.25f, LayerMask.GetMask("Scenery")))
        {
            
            return true;
        }

            return false;
        
    }

    public void SimulateMovement(List<Boid> other, float time, Vector3 roamingDir)
    {
        //default vars
        var steering = roamingDir;

        var separationDirection = Vector3.zero;
        var separationCount = 0;
        var alignmentDirection = Vector3.zero;
        var alignmentCount = 0;
        var cohesionDirection = Vector3.zero;
        var cohesionCount = 0;

        foreach (Boid boid in other)
        {
            //skip self
            if (boid == this)
                continue;

            var distance = Vector3.Distance(boid.transform.position, this.transform.position);

            //identify local neighbour
            if (distance < NoClumpingRadius)
            {
                separationDirection += boid.transform.position - transform.position;
                separationCount++;
            }

            //identify local neighbour
            if (distance < LocalAreaRadius && boid.SwarmIndex == this.SwarmIndex)
            {
                alignmentDirection += boid.transform.forward;
                alignmentCount++;

                cohesionDirection += boid.transform.position - transform.position;
                cohesionCount++;
            }
        }

        if (separationCount > 0)
            separationDirection /= separationCount;

        //flip
        separationDirection = -separationDirection;

        if (alignmentCount > 0)
            alignmentDirection /= alignmentCount;

        if (cohesionCount > 0)
            cohesionDirection /= cohesionCount;

        //get direction to center of mass
        cohesionDirection -= transform.position;

        //weighted rules
        steering += separationDirection.normalized * seperationWeight;
        steering += alignmentDirection.normalized * alignmentWeight;
        steering += cohesionDirection.normalized * cohesionWeight;


        //obstacle avoidance
        /*
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, LocalAreaRadius, LayerMask.GetMask("Scenery")))
            steering = (hitInfo.point + hitInfo.normal - transform.position).normalized;
        */
        if (IsHeadingForCollision(steering))
        {
            steering = FindUnobstructedDirection(roamingDir);
        }


        //apply steering
        //if (steering != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), SteeringSpeed * time);

        Debug.DrawRay(transform.position, transform.TransformDirection(steering), Color.red, 0.5f);

        //move 
        transform.position += transform.TransformDirection(steering) * time * Speed;
    }


}

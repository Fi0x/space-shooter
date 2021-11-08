using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public const int numViewDirections = 300;
    public Vector3[] directions;
    float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;

    // Start is called before the first frame update
    void Start()
    {
        BoidHelper();
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
        BoidHelper();
        float furthestUnobstructedDst = 0;
        RaycastHit hit;

        
        float radius = 1.0f;
        float ViewRadius = 10.0f;
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
        if (Physics.SphereCast(transform.position, 1.0f, direction, out hit, 10.0f, LayerMask.GetMask("Scenery")))
        {
            
            return true;
        }

            return false;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    [SerializeField]
    public List<Boid> boids;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        foreach(BoidController boid in boids)
        {
            boid.SimulateMovement(boids, Time.deltaTime);
        }
        */
    }
}

using System.Collections;
using System.Collections.Generic;
using Ship.Rocket;
using UnityEngine;

public class TempRocketSpawner : MonoBehaviour
{
    [SerializeField] private Rigidbody shipRb;
    public Transform spawnPoint;
    public GameObject prefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Transform spawnTransform = spawnPoint.transform;
            //Debug.Log("spawnRocket");
            var rocket = Instantiate(prefab, spawnTransform.position, spawnTransform.rotation);
            float speed = Mathf.Min(Vector3.Dot(shipRb.velocity.normalized, spawnTransform.forward) * shipRb.velocity.magnitude, 0f);
            rocket.GetComponent<Rigidbody>().velocity = speed * spawnTransform.forward;
        }
    }
}

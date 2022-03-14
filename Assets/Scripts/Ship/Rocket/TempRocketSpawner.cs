using System.Collections;
using System.Collections.Generic;
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
            //Debug.Log("spawnRocket");
            var rocket = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            rocket.GetComponent<Rigidbody>().velocity = shipRb.velocity;
        }
    }
}

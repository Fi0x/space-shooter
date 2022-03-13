using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempRocketSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject prefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("spawnRocket");
            Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }
}

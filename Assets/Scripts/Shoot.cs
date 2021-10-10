using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("Projectile")]
    //
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform attackPoint;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(projectilePrefab, attackPoint.position, transform.rotation);
        }
    }
}

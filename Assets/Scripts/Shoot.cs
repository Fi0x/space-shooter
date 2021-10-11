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

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody ship;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var projectile = Instantiate(projectilePrefab, attackPoint.position, transform.rotation);

            var projectileDirectionAndVelocity = ship.velocity + attackPoint.forward * speed;
            projectile.GetComponent<SphereProjectile>().InitializeDirection(projectileDirectionAndVelocity, true);

        }
    }
}

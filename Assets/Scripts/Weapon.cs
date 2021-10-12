using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float speed = 100f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private AnimationCurve damageOverTime;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Rigidbody ship;
    [SerializeField] private WeaponManager weaponManager;

    private bool isShooting = false;
    private float timeSinceLastFire = 0f;

    private UnityAction<bool> firemodeChangedEvent;

    // Start is called before the first frame update
    private void Start()
    {
        this.firemodeChangedEvent += FiremodeChangedEventHandler;
        this.weaponManager.FiremodeChangedEvent.AddListener(this.firemodeChangedEvent);
    }

    private void FiremodeChangedEventHandler(bool newFireMode)
    {
        this.isShooting = newFireMode;
        Debug.Log(newFireMode);
        if (newFireMode)
        {
            this.Fire();
        }
        else
        {
            this.timeSinceLastFire = 0f;
        }
    }

    private void OnDestroy()
    {
        this.weaponManager.FiremodeChangedEvent.RemoveListener(this.firemodeChangedEvent);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isShooting)
        {
            if (timeSinceLastFire > fireRate)
            {
                this.Fire();
                timeSinceLastFire -= fireRate;
            }

            timeSinceLastFire += Time.fixedDeltaTime;
        }


        this.gameObject.transform.LookAt(this.weaponManager.Target, this.transform.parent.gameObject.transform.forward);


    }

    private void Fire()
    {
        var projectile = Instantiate(projectilePrefab, this.gameObject.transform);
        var shotDirection = this.weaponManager.Target - this.gameObject.transform.position;
        var projectileDirectionAndVelocity = this.ship.velocity + shotDirection.normalized * this.speed;
        projectile.GetComponent<SphereProjectile>().InitializeDirection(projectileDirectionAndVelocity, true, this.damageOverTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using Components;
using Enemy;
using Manager;
using Ship.Weaponry.Config;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("ProjectileSettings")]
    public GameObject projectilePrefab;
    public GameObject muzzleVfx;
    [SerializeField] private float fireRate = 2.5f;
    [SerializeField] private Transform gunPoint;
    
    [Header("PlayerDetection")]
    public Transform targetTransform;
    public GameObject player;
    [SerializeField] private float attackRange = 200f;
    [SerializeField] private float turnTime = 0.3f;
    [SerializeField] private float angleOfAttack = 7f;

    [Header("Settings")]
    [SerializeField] private int maxHealth = 3000;
    
    private Vector3 smoothVel = Vector3.zero;
    private bool canShoot = true;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.Player;
        GetComponent<Health>().MaxHealth = maxHealth;
        GameManager.Instance.EnemyManager.NotifyAboutNewEnemySpawned(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null) return;
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist > attackRange) return;
        targetTransform.position =
            Vector3.SmoothDamp(targetTransform.position, player.transform.position, ref smoothVel, turnTime);
        CheckAngle();
    }

    private void CheckAngle()
    {
        Vector3 desiredTargetDir = (player.transform.position - gunPoint.transform.position).normalized;
        Vector3 actualTargetDir = gunPoint.forward;
        float angle = Vector3.Angle(desiredTargetDir, actualTargetDir);
        if (angle <= angleOfAttack) Attack();
    }

    private void Attack()
    {
        if(!canShoot) return;
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        SpawnProjectile();
        yield return new WaitForSeconds(1f / fireRate);
        canShoot = true;
    }

    private void SpawnProjectile()
    {
        var muzzle = Instantiate(muzzleVfx, gunPoint);
        muzzle.transform.position = gunPoint.transform.position;
        muzzle.transform.rotation = gunPoint.transform.rotation;
        var projectile = Instantiate(projectilePrefab, gunPoint.transform.position, gunPoint.rotation);
        EnemyProjectile eP = projectile.GetComponent<EnemyProjectile>();
        eP.speed = 50f;
        eP.Damage = 10;
        eP.timeToLive = 10f;
    }
}

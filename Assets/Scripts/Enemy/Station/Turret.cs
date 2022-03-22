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

    private Vector3 predictedTarget;
    private Vector3 smoothVel = Vector3.zero;
    private bool canShoot = true;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.Player;
        GetComponent<Health>().MaxHealth = maxHealth;
        //GameManager.Instance.EnemyManager.NotifyAboutNewEnemySpawned(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // if(player == null) return;
        // UpdateTarget();
        StartCoroutine(UpdateEvery(0.3f));
    }

    IEnumerator UpdateEvery(float secs)
    {
        for (;;)
        {
            if(player != null)
                UpdateTarget();
            yield return new WaitForSeconds(secs);
        }
    }

    private void UpdateTarget()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist > attackRange) return;
        predictedTarget = PredictTarget();
        targetTransform.position =
            Vector3.SmoothDamp(targetTransform.position, predictedTarget, ref smoothVel, turnTime);
        CheckAngle();
    }

    private Vector3 PredictTarget()
    {
        var playerPos = player.transform.position;
        playerPos += (0.01f * player.GetComponent<Rigidbody>().velocity) * Vector3.Distance(playerPos, gunPoint.position);
        Debug.DrawLine(gunPoint.position, predictedTarget);
        
        return playerPos;
    }

    private void CheckAngle()
    {
        Vector3 desiredTargetDir = (predictedTarget - gunPoint.transform.position).normalized;
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
        eP.direction = gunPoint.forward;
    }
}

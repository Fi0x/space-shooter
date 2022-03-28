using System.Collections;
using UnityEngine;

namespace Enemy.Station
{
    public class Turret : TurretBase
    {
        [Header("ProjectileSettings")]
        public GameObject projectilePrefab;
        public GameObject muzzleVfx;
        [SerializeField] private float fireRate = 2.5f;
        [SerializeField] private float projectileSpeed = 75f;
        [SerializeField] private int damage = 10;
        private bool canShoot = true;
        private Rigidbody playerRb;
    
        protected override Vector3 PredictTarget()
        {
            if (playerRb == null)
                playerRb = player.GetComponent<Rigidbody>();
            var playerPos = player.transform.position;
            playerPos += playerRb.velocity * (0.01f * Vector3.Distance(playerPos, gunPoint.position));
            Debug.DrawLine(gunPoint.position, predictedTarget);
                
            return playerPos;
        }

        protected override void Attack()
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
            var gunPointTransform = gunPoint.transform;
            muzzle.transform.position = gunPointTransform.position;
            muzzle.transform.rotation = gunPointTransform.rotation;
            var projectile = Instantiate(projectilePrefab, gunPointTransform.position, gunPoint.rotation);
            EnemyProjectile eP = projectile.GetComponent<EnemyProjectile>();
            eP.speed = projectileSpeed;
            eP.Damage = damage;
            eP.timeToLive = 10f;
            eP.direction = gunPoint.forward;
        }
    }
}

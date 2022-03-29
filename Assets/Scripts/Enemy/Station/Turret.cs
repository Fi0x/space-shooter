using System.Collections;
using Targeting;
using UnityEngine;

namespace Enemy.Station
{
    public class Turret : TurretBase
    {
        [Header("ProjectileSettings")]
        public GameObject projectilePrefab;
        public GameObject muzzleVfx;
        [SerializeField] private float fireRate = 2.5f;
        [SerializeField] private float projectileSpeed = 250f;
        [SerializeField] private int damage = 10;
        private bool canShoot = true;
        private Rigidbody playerRb;
    
        protected override Vector3 PredictTarget()
        {
            if (playerRb == null)
                playerRb = player.GetComponent<Rigidbody>();
            var shooterPos = gunPoint.transform.position;
            var playerPos = player.transform.position;
            var playerVelocity = playerRb.velocity;
            var t = TargetingCalculationHelper.GetPredictedTimeOfCollision(gunPoint.transform.position, projectileSpeed,
                playerPos, playerVelocity);
            if (t.HasValue)
            {
                var predictedPosition = playerPos + playerVelocity * t.Value;
                Debug.DrawLine(gunPoint.position, predictedPosition, Color.green);
            
                return predictedPosition;
            }
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
            eP.timeToLive = 4f;
            eP.direction = gunPoint.forward;
        }
    }
}

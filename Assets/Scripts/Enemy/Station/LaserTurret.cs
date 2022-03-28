using System;
using System.Collections;
using Components;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace Enemy.Station
{
    public class LaserTurret : TurretBase
    {
        [Header("LaserSettings")]
        public VisualEffect laserEffect;
        public VisualEffect trackingLaser;
        [SerializeField] private float damage = 150f;
        [SerializeField] private float fireDelay = 3f;
        [SerializeField] private float timeBetweenShots = 1f;
        [SerializeField] private float range = 400f;
        [SerializeField] private float width = 1f;
        [SerializeField] private LayerMask mask;
        [SerializeField] private GameObject impactPrefab;
 
        private bool canShoot = true;
        private float timeUntilShot = 0f;

        private void Awake()
        {
            timeUntilShot = fireDelay;
        }

        protected override Vector3 PredictTarget()
        {
            Vector3 pos = player.transform.position + player.GetComponent<Rigidbody>().velocity * timeUntilShot;
            return pos;
        }

        protected override void Attack()
        {
            if(canShoot)
                StartCoroutine(SequenceAttack());
        }

        IEnumerator SequenceAttack()
        {
            timeUntilShot = fireDelay;
            canShoot = false;
            var oldTurnTime = turnTime;
            turnTime = turnTime * 4f;
            //start trackingLaser
            trackingLaser.SetFloat("length", range);
            trackingLaser.Play();

            while (timeUntilShot > 0f)
            {
                timeUntilShot -= Time.deltaTime;
                yield return null;
            }
            
            lockRotation = true;
            trackingLaser.Stop();
            //damageLaser
            laserEffect.SetFloat("Length", range);
            laserEffect.SetFloat("Width", width);
            laserEffect.Play();
            DamageInLine();
            
            yield return new WaitForSeconds(timeBetweenShots);
            canShoot = true;
            turnTime = oldTurnTime;
            lockRotation = false;
            timeUntilShot = fireDelay;
        }

        private void DamageInLine()
        {
            var hits = Physics.SphereCastAll(gunPoint.position, width, gunPoint.forward, range, mask);
            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage);
                }
                var impact = Instantiate(impactPrefab, hit.point, Quaternion.identity);
                Destroy(impact, 3f);
            }
        }
    }
}

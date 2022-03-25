using System;
using System.Collections;
using Manager;
using UnityEngine;
using UpgradeSystem;

namespace Ship.Rocket
{
    public class RocketSpawner : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InputHandler input;
        [SerializeField] private Rigidbody shipRb;
        public Transform spawnPoint;
        public GameObject prefab;
        public UpgradeDataSO upgradeData;

        [Header("Stats")]
        [SerializeField] public int maxRocketCharges = 3;
        [SerializeField][ReadOnlyInspector] public int currentCharges;
        [SerializeField] private float rechargeTime = 3f;
        [SerializeField][ReadOnlyInspector] public float chargePct;
        [SerializeField] private float fireDelay = .1f;
        
        private Coroutine chargingCoroutine;
        private bool canFire = true;

        private void Start()
        {
            currentCharges = maxRocketCharges;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Fire();
            }

            UpdateRocketCharging();
        }

        public float CalcRechargeTime()
        {
            return rechargeTime * (1f / upgradeData.GetValue(UpgradeNames.RocketChargeSpeed));
        }

        public int CalcRocketCharges()
        {
            return maxRocketCharges + (int)upgradeData.GetValue(UpgradeNames.MaxRockets);
        }

        private void UpdateRocketCharging()
        {
            if (currentCharges >= CalcRocketCharges())
            {
                //StopCoroutine(chargingCoroutine);
                return;
            }
            if (chargingCoroutine != null) return;
            chargingCoroutine = StartCoroutine(ChargeRocket());
        }

        IEnumerator ChargeRocket()
        {
            float currentTime = 0f;
            while (currentTime <= CalcRechargeTime())
            {
                currentTime += Time.deltaTime;
                chargePct = currentTime / CalcRechargeTime();
                yield return null;
            }
            currentCharges++;
            chargePct = 0f;
            AudioManager.instance.Play("RocketCharged");
            chargingCoroutine = null;
        }

        private void Fire()
        {
            if(currentCharges <= 0) return;
            if (!canFire) return;
            SpawnRocket();
        }

        IEnumerator ResetFire()
        {
            yield return new WaitForSeconds(fireDelay);
            canFire = true;
        }
        
        private void SpawnRocket()
        {
            //reset
            canFire = false;
            StartCoroutine(ResetFire());
            currentCharges--;
            
            //spawn rocket
            Transform spawnTransform = spawnPoint.transform;
            var rocket = Instantiate(prefab, spawnTransform.position, spawnTransform.rotation);
            float speed = Mathf.Min(Vector3.Dot(shipRb.velocity.normalized, spawnTransform.forward) * shipRb.velocity.magnitude, 0f);
            rocket.GetComponent<Rigidbody>().velocity = speed * spawnTransform.forward;
        }
    }
}

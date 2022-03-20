using System;
using System.Collections;
using UnityEngine;

namespace Ship.Rocket
{
    public class RocketSpawner : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InputHandler input;
        [SerializeField] private Rigidbody shipRb;
        public Transform spawnPoint;
        public GameObject prefab;

        [Header("Stats")]
        [SerializeField] private int maxRocketCharges = 3;
        [SerializeField][ReadOnlyInspector] private int currentCharges;
        [SerializeField] private float rechargeTime = 3f;
        [SerializeField][ReadOnlyInspector] private float chargePct;
        [SerializeField] private float fireDelay = 1f;
        
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

        private void UpdateRocketCharging()
        {
            if (currentCharges >= maxRocketCharges)
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
            while (currentTime <= rechargeTime)
            {
                currentTime += Time.deltaTime;
                chargePct = currentTime / rechargeTime;
                yield return null;
            }
            currentCharges++;
            chargePct = 0f;
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

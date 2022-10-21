using System;
using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using UpgradeSystem;

namespace Ship.Rocket
{
    public class RocketSpawner : MonoBehaviour
    {
        [Header("Dependencies")]
         private InputMap input;
        [SerializeField] private Rigidbody shipRb;
        public Transform spawnPoint;
        public GameObject prefab;
        public UpgradeDataSO upgradeData;

        [Header("Stats")]
        [SerializeField] public int maxRocketCharges = 3;
        [SerializeField][ReadOnlyInspector] public int currentCharges;
        [SerializeField] private float rechargeTime = 15f;
        [SerializeField][ReadOnlyInspector] public float chargePct;
        [SerializeField] private float fireDelay = .1f;
        
        private Coroutine chargingCoroutine;
        private bool canFire = true;

        private void OnEnable()
        {
            input = new InputMap();
            input.Player.Enable();
            input.Player.AltShoot.performed += Fire;
        }

        private void OnDisable()
        {
            input.Player.Disable();
            input.Player.AltShoot.performed -= Fire;
        }

        private void Start()
        {
            currentCharges = maxRocketCharges;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRocketCharging();
        }

        public float CalcRechargeTime()
        {
            //return rechargeTime * (1f / (0.25f * upgradeData.GetValue(UpgradeNames.RocketChargeSpeed) + 1f));
            return upgradeData.GetValue(UpgradeNames.RocketChargeSpeed);
        }

        public int CalcRocketCharges()
        {
            //return maxRocketCharges + (int)upgradeData.GetValue(UpgradeNames.MaxRockets);
            return (int) upgradeData.GetValue(UpgradeNames.MaxRockets);
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

        private void Fire(InputAction.CallbackContext ctx)
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
            var shipVelocity = shipRb.velocity.magnitude;
            var rocket = Instantiate(prefab, spawnTransform.position, spawnTransform.rotation);
            rocket.GetComponent<SeekingRocket>().maxSpeed =
                rocket.GetComponent<SeekingRocket>().maxSpeed + Mathf.Min(shipVelocity, 0f);
            float speed = Mathf.Min(Vector3.Dot(shipRb.velocity.normalized, spawnTransform.forward) * shipRb.velocity.magnitude, 0f);
            rocket.GetComponent<Rigidbody>().velocity = speed * spawnTransform.forward;
        }
    }
}

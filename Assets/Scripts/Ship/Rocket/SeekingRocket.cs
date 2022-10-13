﻿using System;
using System.Collections;
using Components;
using UnityEngine;
using UnityEngine.VFX;

namespace Ship.Rocket
{
    public class SeekingRocket : MonoBehaviour
    {
        [Header("RocketTargeting")]
        [SerializeField] private float seekingRadius = 200f;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private Transform target;
        [SerializeField] private float timeBetweenUpdates = 0.5f;

        [Header("Movement")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] public float maxSpeed = 30f;
        [SerializeField] private float turnSpeed = 3f;
        [SerializeField] private float inertia = 0.1f;

        [Header("Stats")]
        [SerializeField] private float explosionRadius;
        [SerializeField] private float damage;
        [SerializeField] private AnimationCurve damageFalloff;
        [SerializeField] private float maxLifetime;

        [Header("VFX")]
        [SerializeField] private GameObject vfxPrefab;
        [SerializeField] private float vfxLifetime = 5f;

        private Vector3 dampVel;

        private void Start()
        {
            maxSpeed += rb.velocity.magnitude;
            StartCoroutine(UpdateTarget());
            StartCoroutine(ExplodeIn(maxLifetime));
        }

        private void FixedUpdate()
        {
            SteerTowardsTarget();
        }

        private void SteerTowardsTarget()
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, transform.forward * maxSpeed,  ref dampVel, inertia);
            if (target == null) return;
            Vector3 predicted = PredictTarget();
            Debug.DrawLine(transform.position, predicted, Color.yellow);
            var rocketRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(predicted - transform.position), turnSpeed);
            
            rb.MoveRotation(rocketRotation);
        }

        private Vector3 PredictTarget()
        {
            Vector3 predictedTarget = target.position;
            if (target.TryGetComponent(out Rigidbody targetRb))
            {
                if (rb.velocity.magnitude == 0) return predictedTarget;
                predictedTarget += targetRb.velocity * (Vector3.Distance(predictedTarget, transform.position) / rb.velocity.magnitude);
            }
            return predictedTarget;
        }

        private void OnCollisionEnter(Collision other)
        {
            Explode();
        }

        private void Explode()
        {
            //spawn vfx
            var vfxObj = Instantiate(vfxPrefab, transform.position, transform.rotation);
            var vfx = vfxObj.GetComponent<VisualEffect>();
            vfx.SetFloat("Circumference", explosionRadius * 2f);
            Destroy(vfxObj, vfxLifetime);
            
            //deal damage
            DamageTargetsInRadius();
            
            //kill rocket
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private IEnumerator UpdateTarget()
        {
            for (;;)
            {
                target = CheckForTarget();
                if(target !=null)
                    Debug.DrawLine(transform.position, target.position, Color.green);
                yield return new WaitForSeconds(timeBetweenUpdates);
            }
        }

        private IEnumerator ExplodeIn(float secs)
        {
            yield return new WaitForSeconds(secs);
            Explode();
        }

        private void DamageTargetsInRadius()
        {
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius, targetMask);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IDamageable damageable))
                {
                    float normalizedDist = Vector3.Distance(collider.gameObject.transform.position, transform.position) / explosionRadius;
                    float falloffMultiplier = damageFalloff.Evaluate(normalizedDist);
                    damageable.TakeDamage(damage * falloffMultiplier);
                }
            }
        }
        
        private Transform CheckForTarget()
        {
            var colliders = Physics.OverlapSphere(transform.position, seekingRadius, targetMask);
            var newTarget = GetClosestEnemy(colliders);
            if (newTarget != null) return newTarget;
            return null;
        }
        
        Transform GetClosestEnemy (Collider[] enemyColliders)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach(var collider in enemyColliders)
            {
                //Debug.Log(collider.gameObject);
                var potentialTarget = collider.gameObject.transform;
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
     
            //Debug.Log(bestTarget);
            return bestTarget;
        }
    }
}
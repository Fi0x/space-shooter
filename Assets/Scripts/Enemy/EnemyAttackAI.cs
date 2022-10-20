using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackAI : MonoBehaviour
    {
        public List<Transform> attackPoints;
        public EnemyAISO enemySettings;
        public Transform target;
        
        private int index = 0;
        private bool canAttack = true;

        private void Update()
        {
            TryAttack();
        }

        public void TryAttack()
        {
            if (target == null) target = GameManager.Instance.Player.transform;
            if(!canAttack) return;
            Vector3 predictedPosition = target.position + target.GetComponent<Rigidbody>().velocity.magnitude * target.forward;
            Vector3 toTarget = (predictedPosition - transform.position).normalized;
            if(Mathf.Abs(Vector3.Angle(transform.forward, toTarget)) > enemySettings.attackAngle) return;
            Attack();
            StartCoroutine(ResetAttack());
        }

        private void Attack()
        {
            var projectile = Instantiate(enemySettings.projectilePrefab);
            projectile.transform.SetPositionAndRotation(attackPoints[index].position, attackPoints[index].rotation);
            projectile.GetComponent<EnemyProjectile>().direction = attackPoints[index].forward;
            index = (index + 1) % attackPoints.Count;
        }

        private IEnumerator ResetAttack()
        {
            canAttack = false;
            yield return new WaitForSeconds(1f / enemySettings.attackSpeed);
            canAttack = true;
        }
    }
}
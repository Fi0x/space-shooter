using System;
using System.Collections;
using HealthSystem;
using Manager;
using Stats;
using UnityEngine;

namespace Enemy.Station
{
    public abstract class TurretBase : MonoBehaviour
    {
        [SerializeField] protected Transform gunPoint;
        
        [Header("PlayerDetection")]
        public Transform targetTransform;
        public GameObject player;
        [SerializeField] protected float attackRange = 200f;
        [SerializeField] protected float turnTime = 0.3f;
        [SerializeField] protected float angleOfAttack = 7f;
        [SerializeField] protected bool lockRotation = false;

        [Header("Settings")]
        [SerializeField] private int maxHealth = 3000;

        protected Vector3 predictedTarget;
        protected Vector3 smoothVel = Vector3.zero;
        
        // Start is called before the first frame update
        void Start()
        {
            player = GameManager.Instance.Player;
            GetComponent<Health>().MaxHealth = maxHealth;
            GameManager.Instance.EnemyLevelCounter++;
        }
    
        // Update is called once per frame
        void Update()
        {
            if(player == null) return;
            UpdateTarget();
            CheckAngle();
        }
    
        private void UpdateTarget()
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist > attackRange) return;
            if (!lockRotation)
            {
                predictedTarget = PredictTarget();
                targetTransform.position =
                    Vector3.SmoothDamp(targetTransform.position, predictedTarget, ref smoothVel, turnTime);
            }
        }

        protected virtual Vector3 PredictTarget()
        {
            return player.transform.position;
        }
    
        private void CheckAngle()
        {
            Vector3 desiredTargetDir = (predictedTarget - gunPoint.transform.position).normalized;
            Vector3 actualTargetDir = gunPoint.forward;
            float angle = Vector3.Angle(desiredTargetDir, actualTargetDir);
            if (angle <= angleOfAttack) Attack();
        }

        protected virtual void Attack(){}

        private void OnDestroy()
        {
            StatCollector.UpdateGeneralStat("Enemies Killed", 1);
            GameManager.Instance.playerUpgrades.freePoints++;
            GameManager.Instance.DestroyedEnemyLevelCounter++;
            this.PostOnDestroy();
        }

        protected virtual void PostOnDestroy(){}
    }
}
using System;
using Manager;
using Ship.Movement;
using Ship.Sensors;
using UnityEngine;

namespace Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI")]
        [SerializeField] [ReadOnlyInspector] private State state;
        [SerializeField] private Transform[] attackPoints;
        [SerializeField] private NpcShipMovementHandler shipMovementHandler;

        [Header("Roaming")]
        [SerializeField] [ReadOnlyInspector] public Vector3 roamingPosition;
        [SerializeField] private float reachedPositionMaxDistance;

        [Header("Attack")]
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] [ReadOnlyInspector] private float waitForAttack;
        [SerializeField] private GameObject projectilePrefab;

        [Header("Ranges")]
        [SerializeField] private float sightRange;
        [SerializeField] private float attackRange;

        [Header("Boid")]
        [SerializeField] private Boid boid;
        [SerializeField] private BoidController boidController;

        private SensorTarget target;

        public BoidController Flock => this.boidController;

        public void InitializeEnemyAI(BoidController controller)
        {
            this.boid = this.GetComponent<Boid>();
            this.shipMovementHandler ??= this.GetComponent<NpcShipMovementHandler>() ??
                                         throw new NullReferenceException("No NpcShipMovementHandler set or found");

            this.state = State.Roaming;
            this.reachedPositionMaxDistance = 20.0f;

            this.waitForAttack = 2.0f;
            this.timeBetweenAttacks = this.waitForAttack;

            this.boidController = controller;
            this.target = this.GetComponent<SensorTarget>();
        }

        private void Update()
        {
     
            switch (this.state)
            {
                case State.Roaming:
                    this.RoamAround();
                    break;
                case State.ChasePlayer:
                    this.ChasePlayer();
                    break;
                case State.AttackPlayer:
                    this.AttackPlayer();
                    break;
            }

            var updatedState = this.UpdateState();
            if (updatedState == this.state)
                return;
            
            this.state = updatedState;
            switch (updatedState)
            {
                case State.ChasePlayer:
                    this.target.UpdateAllegiance(SensorTarget.TargetAllegiance.Hostile);
                    break;
                case State.AttackPlayer:
                    this.target.UpdateAllegiance(SensorTarget.TargetAllegiance.Aggressive);
                    break;
                case State.Roaming:
                    this.target.UpdateAllegiance(SensorTarget.TargetAllegiance.Neutral);
                    break;
            }
        }

        private void ChasePlayer()
        {
            var playerPos = GameManager.Instance.Player.transform.position;
            this.shipMovementHandler.NotifyAboutNewLookAtTarget(playerPos);
            var direction = playerPos - this.transform.position;
            this.shipMovementHandler.NotifyAboutNewTargetDirectionWithVelocity(direction.normalized * this.boid.DesiredSpeed);
        }

        private void AttackPlayer()
        {
            this.shipMovementHandler.NotifyAboutNewTargetDirectionWithVelocity(Vector3.zero);
            this.shipMovementHandler.NotifyAboutNewLookAtTarget(GameManager.Instance.Player.transform.position);

            // TODO: Replace with weapon trigger
            this.waitForAttack -= Time.deltaTime;
            var angleBetweenSelfAndPlayer = Vector3.Angle(this.transform.forward,
                GameManager.Instance.Player.transform.position - this.transform.position);
            if (this.waitForAttack < 0f
                && angleBetweenSelfAndPlayer < 10)
            {
                this.waitForAttack = this.timeBetweenAttacks;
                foreach (var attackPoint in this.attackPoints)
                {
                    var projectile = Instantiate(this.projectilePrefab, attackPoint.position, this.transform.rotation);
                    EnemyProjectile eP = projectile.GetComponent<EnemyProjectile>();
                    eP.speed = 50f;
                    eP.Damage = 25;
                    eP.timeToLive = 5f;
                    eP.direction = attackPoint.forward;
                }
            }
        }

        private void RoamAround()
        {
            if(Vector3.Distance(this.transform.position, this.roamingPosition) < this.reachedPositionMaxDistance)
                this.boidController.SetNewRoamingPosition();
                
            var desiredMovementDirection = this.boid.GetDesiredDirectionAndVelocity();
            this.shipMovementHandler.NotifyAboutNewLookAtTarget(null);
            this.shipMovementHandler.NotifyAboutNewTargetDirectionWithVelocity(desiredMovementDirection);
        }

        private State UpdateState()
        {
            var distanceBetweenSelfAndPlayer =
                Vector3.Distance(this.transform.position, GameManager.Instance.Player.transform.position);
            if (distanceBetweenSelfAndPlayer <= this.attackRange)
                return State.AttackPlayer;
            
            
            return distanceBetweenSelfAndPlayer <= this.sightRange
                ? this.state = State.ChasePlayer
                : this.state = State.Roaming;
        }

        [Obsolete]
        private void FaceTarget(Vector3 lookDirection)
        {
            var direction = (lookDirection - this.transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    
        private enum State
        {
            Roaming,
            ChasePlayer,
            AttackPlayer,
        }
    }
}
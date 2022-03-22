using Manager;
using Ship;
using UnityEngine;

namespace Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI")]
        [SerializeField] [ReadOnlyInspector] private State state;
        [SerializeField] private Transform[] attackPoints;

        [Header("Roaming")]
        [SerializeField] [ReadOnlyInspector] public Vector3 roamingPosition;
        [SerializeField] private float reachedPositionMaxDistance;
        [SerializeField] private float speed;

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

        public void InitializeEnemyAI(BoidController controller)
        {
            this.boid = this.GetComponent<Boid>();

            this.state = State.Roaming;
            this.reachedPositionMaxDistance = 20.0f;
            //this.speed = ShipMovementHandler.TotalMaxSpeed * 0.8f;
            this.speed = 20f; // TODO

            this.waitForAttack = 2.0f;
            this.timeBetweenAttacks = this.waitForAttack;
            this.sightRange = 125.0f;
            this.attackRange = 75.0f;

            this.boidController = controller;
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
        }

        private void ChasePlayer()
        {
            this.FaceTarget(GameManager.Instance.Player.transform.position);
            this.transform.position = Vector3.MoveTowards(
                this.transform.position,
                GameManager.Instance.Player.transform.position,
                this.speed / 4 * Time.deltaTime);
        
            this.CheckState();
        }

        private void AttackPlayer()
        {
            this.FaceTarget(GameManager.Instance.Player.transform.position);

            this.waitForAttack -= Time.deltaTime;
            if (this.waitForAttack < 0f
                && Vector3.Angle(this.transform.forward, GameManager.Instance.Player.transform.position - this.transform.position) < 10)
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

            this.CheckState();
        }

        private void RoamAround()
        {
            if(Vector3.Distance(this.transform.position, this.roamingPosition) < this.reachedPositionMaxDistance)
                this.boidController.SetNewRoamingPosition();
                
            this.boid.MoveUnit();
            this.CheckState();
        }

        private void CheckState()
        {
            this.state = Vector3.Distance(this.transform.position, GameManager.Instance.Player.transform.position) <= this.sightRange
                ? this.state = State.ChasePlayer
                : this.state = State.Roaming;

            if (Vector3.Distance(this.transform.position, GameManager.Instance.Player.transform.position) <= this.attackRange)
                this.state = State.AttackPlayer;
        }

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
using System.Collections.Generic;
using Manager;
using Ship;
using UnityEngine;

namespace Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI")]
        [SerializeField] [ReadOnlyInspector] private State state;
        [SerializeField] private List<Transform> attackPoints;

        [Header("Roaming")]
        [SerializeField] [ReadOnlyInspector] private Vector3 roamingPosition;
        [SerializeField] private float reachedPositionMaxDistance;
        [SerializeField] private float minDistanceFromPlayer;
        [SerializeField] private float maxDistanceFromPlayer;
        [SerializeField] private float speed;

        [Header("Attack")]
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] [ReadOnlyInspector] private float waitForAttack;
        [SerializeField] private GameObject projectilePrefab;

        [Header("Ranges")]
        [SerializeField] private float sightRange;
        [SerializeField] private float attackRange;

        private void Start()
        {
            // Start in RoamState
            this.state = State.Roaming;

            // Patrolling
            this.minDistanceFromPlayer = 75.0f;
            this.maxDistanceFromPlayer = 250.0f;
            this.roamingPosition = this.GetRoamingPosition();

            this.reachedPositionMaxDistance = 2.0f;

            this.speed = 20.0f;

            // Attack
            this.waitForAttack = 2.0f;
            this.timeBetweenAttacks = this.waitForAttack;

            // Ranges
            this.sightRange = 125.0f;
            this.attackRange = 75.0f;

            FlightModel.FlightModelChangedEvent += (sender, args) => { this.speed = args.NewMaxSpeed * 0.8f; };
        }

        private void Update()
        {
            switch (this.state)
            {
                case State.Roaming:
                    // rotate towards position
                    this.FaceTarget(this.roamingPosition);
                    // move towards position
                    this.transform.position = Vector3.MoveTowards(this.transform.position, this.roamingPosition, this.speed * Time.deltaTime);
                    // check if roamingPosition has been reached
                    if (Vector3.Distance(this.transform.position, this.roamingPosition) < this.reachedPositionMaxDistance)
                    {
                        // determine a new roamPosition
                        this.roamingPosition = this.GetRoamingPosition();
                    }
                    this.CheckState();
                    break;
                case State.ChasePlayer:
                    // rotate towards position
                    var playerPos = GameManager.Instance.Player.transform.position;
                    this.FaceTarget(playerPos);
                    // move towards Player
                    this.transform.position = Vector3.MoveTowards(this.transform.position, playerPos, this.speed * Time.deltaTime);
                    this.CheckState();
                    break;
                case State.AttackPlayer:
                    // rotate towards Player
                    this.FaceTarget(GameManager.Instance.Player.transform.position);
                    // Attack
                    this.waitForAttack -= Time.deltaTime;
                    if (this.waitForAttack < 0f && IsFacingTarget(this.transform.forward, GameManager.Instance.Player.transform.position - this.transform.position, 10))
                    {
                        this.waitForAttack = this.timeBetweenAttacks;
                        this.Attack();
                    }
                    this.CheckState();
                    break;
            }
        }

        private void CheckState()
        {
            // Check for sightRange
            this.state = Vector3.Distance(this.transform.position, GameManager.Instance.Player.transform.position) <= this.sightRange
                ? State.ChasePlayer
                : State.Roaming;

            // Check for attackRange
            if (Vector3.Distance(this.transform.position, GameManager.Instance.Player.transform.position) <= this.attackRange)
            {
                this.state = State.AttackPlayer;
            }
        }

        private void FaceTarget(Vector3 lookDirection)
        {
            var direction = (lookDirection - this.transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        private static bool IsFacingTarget(Vector3 forward, Vector3 direction, float tolerance)
        {
            return Vector3.Angle(forward, direction) < tolerance;
        }

        private Vector3 GetRoamingPosition()
        {
            return GameManager.Instance.Player.transform.position + GetRandomDir() * Random.Range(this.minDistanceFromPlayer, this.maxDistanceFromPlayer);
        }

        private static Vector3 GetRandomDir()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        private void Attack()
        {
            foreach (var attackPoint in this.attackPoints)
            {
                Instantiate(this.projectilePrefab, attackPoint.position, this.transform.rotation);
            }
        }
    
        private enum State
        {
            Roaming,
            ChasePlayer,
            AttackPlayer,
        }
    }
}
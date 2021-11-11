using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Roaming,
        ChasePlayer,
        AttackPlayer,
    }

    [Header("AI")]
    //
    [SerializeField] [ReadOnlyInspector] private State state;
    [SerializeField] private Transform AttackPoint;

    // Patroling
    [Header("Roaming")]
    //
    [SerializeField] [ReadOnlyInspector] private Vector3 roamingPosition;
    [SerializeField] float reachedPositionMaxDistance;
    [SerializeField] private float minDistanceFromPlayer;
    [SerializeField] private float maxDistanceFromPlayer;
    [SerializeField] private float speed;

    // Attacking
    [Header("Attack")]
    //
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] [ReadOnlyInspector] private float waitForAttack;
    [SerializeField] private GameObject projectilePrefab;

    // Ranges
    [Header("Ranges")]
    //
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        // get Player
        //this.Player = GameObject.Find("Player");

        // get AttackPoint
        //this.transform.Find("AttackPoint");

        // Start in RoamState
        this.state = State.Roaming;

        // Patroling
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
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.state)
        {
            case State.Roaming:

                // rotate towards position
                this.FaceTarget(this.roamingPosition);
                // move towards position
                this.transform.position =
                    Vector3.MoveTowards(this.transform.position, this.roamingPosition, this.speed * Time.deltaTime);

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
                this.FaceTarget(GameManager.Instance.Player.transform.position);
                // move towards Player
                this.transform.position =
                    Vector3.MoveTowards(this.transform.position, GameManager.Instance.Player.transform.position, this.speed * Time.deltaTime);

                this.CheckState();
                break;


            case State.AttackPlayer:

                // rotate towards Player
                this.FaceTarget(GameManager.Instance.Player.transform.position);

                // Attack
                this.waitForAttack -= Time.deltaTime;
                if (this.waitForAttack < 0f
                    && this.IsFacingTarget(this.transform.forward, GameManager.Instance.Player.transform.position - this.transform.position,
                        10))
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
        if (Vector3.Distance(this.transform.position, GameManager.Instance.Player.transform.position) <= this.sightRange)
        {
            this.state = State.ChasePlayer;
        }
        else
        {
            this.state = State.Roaming;
        }

        // Check for attackRange
        if (Vector3.Distance(this.transform.position, GameManager.Instance.Player.transform.position) <= this.attackRange)
        {
            this.state = State.AttackPlayer;
        }
    }

    private void FaceTarget(Vector3 lookDirection)
    {
        Vector3 direction = (lookDirection - this.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private float GetAngle(Vector3 first, Vector3 second)
    {
        return Vector3.Angle(first, second);
    }

    private bool IsFacingTarget(Vector3 forward, Vector3 direction, float tolerance)
    {
        return Vector3.Angle(forward, direction) < tolerance ? true : false;
    }

    private Vector3 GetRoamingPosition()
    {
        return GameManager.Instance.Player.transform.position + this.GetRandomDir() * Random.Range(this.minDistanceFromPlayer, this.maxDistanceFromPlayer);
    }

    private Vector3 GetRandomDir()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));
    }

    private void Attack()
    {
       Instantiate(this.projectilePrefab, this.AttackPoint.position, this.transform.rotation);
    }
}
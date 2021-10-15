using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    private State state;
    [SerializeField]
    private Transform AttackPoint;
    [SerializeField] 
    private GameObject Player;

    // Patroling
    [Header("Roaming")]
    [SerializeField]
    private Vector3 raomingPosition;
    [SerializeField]
    float reachedPositionMaxDistance;
    [SerializeField]
    private float minDistanceFromPlayer;
    [SerializeField]
    private float maxDistanceFromPlayer;
    [SerializeField]
    private float speed;

    // Attacking
    [Header("Attack")]
    [SerializeField]
    private float timeBetweenAttacks;
    [SerializeField]
    private float waitForAttack;
    [SerializeField]
    private GameObject projectilePrefab;

    // Ranges
    [Header("Ranges")]
    [SerializeField]
    private float sightRange;
    [SerializeField]
    private float attackRange;

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
        minDistanceFromPlayer = 75.0f;
        maxDistanceFromPlayer = 250.0f;
        raomingPosition = GetRoamingPosition();

        reachedPositionMaxDistance = 2.0f;

        speed = 20.0f;

        // Attack
        waitForAttack = 2.0f;
        timeBetweenAttacks = waitForAttack;

        // Ranges
        sightRange = 125.0f;
        attackRange = 75.0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Roaming:

                // rotate towards position
                FaceTarget(raomingPosition);
                // move towards position
                this.transform.position = Vector3.MoveTowards(transform.position, raomingPosition, speed * Time.deltaTime);
                
                // check if roamingPosition has been reached
                if(Vector3.Distance(this.transform.position, raomingPosition) < reachedPositionMaxDistance)
                {
                    // determine a new roamPosition
                    raomingPosition = GetRoamingPosition();
                }

                CheckState();
                break;
                

            case State.ChasePlayer:

                // rotate towards position
                FaceTarget(Player.transform.position);
                // move towards Player
                this.transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, speed * Time.deltaTime);

                CheckState();
                break;


            case State.AttackPlayer:

                // rotate towards Player
                FaceTarget(this.Player.transform.position);

                // Attack
                waitForAttack -= Time.deltaTime;
                if(waitForAttack < 0f 
                    && IsFacingTarget(this.transform.forward, this.Player.transform.position - this.transform.position, 10))
                {
                    waitForAttack = timeBetweenAttacks;

                    Attack();
                }

                CheckState();
                break;
        }
    }

    private void CheckState()
    {
        // Check for sightRange 
        if (Vector3.Distance(this.transform.position, this.Player.transform.position) <= sightRange)
        {
            this.state = State.ChasePlayer;
        }
        else
        {
            this.state = State.Roaming;
        }

        // Check for attackRange
        if (Vector3.Distance(this.transform.position, this.Player.transform.position) <= attackRange)
        {
            this.state = State.AttackPlayer;
        }        
    }

    private void FaceTarget(Vector3 lookDirection)
    {
        Vector3 direction = (lookDirection - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
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
        return this.Player.transform.position + GetRandomDir() * UnityEngine.Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
    }

    private Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
    }

    private void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, AttackPoint.position, transform.rotation);

        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        enemyProjectile.Player = this.Player;
    }
}

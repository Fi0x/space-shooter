using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Helper;
using Manager;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyFlightAI : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float updateFrequency = 0.1f;

    [SerializeField] private EnemyAISO enemySettings;
    
    [SerializeField] private Rigidbody rb;
    private Vector3 desiredDirection;
    private Vector3 targetPosition;
    private Vector3 refer = Vector3.zero;
    private Vector3 homePosition;
    private float desiredSpeed = 1f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    private void Update()
    {
        Debug.DrawLine(transform.position, targetPosition, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + desiredDirection * 20f, Color.magenta);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.EnemyLevelCounter++;
        homePosition = transform.position;
        StartCoroutine(UpdateAll());
    }

    private void FixedUpdate()
    {
        CalculateXRotation();
        CalculateZRotation();
    }

    private void OnDisable()
    {
        StopCoroutine(UpdateAll());
    }

    private void OnDestroy()
    {
        GameManager.Instance.DestroyedEnemyLevelCounter++;
        GameManager.Instance.playerUpgrades.freePoints++;
    }

    IEnumerator UpdateAll()
    {
        for (;;)
        {
            UpdateTarget();
            CheckCollision();
            Move();
            
            yield return new WaitForSeconds(updateFrequency);
        }
    }

    private void CalculateXRotation()
    {
        Vector3 rightVector = transform.right;
        float f = Vector3.Dot(desiredDirection, rightVector);
        transform.RotateAround(transform.position, transform.forward, -f * Time.deltaTime * enemySettings.rollSpeed);
    }

    private void CalculateZRotation()
    {
        Vector3 upVector = transform.up;
        float f = Vector3.Dot(desiredDirection, upVector);
        transform.RotateAround(transform.position, transform.right, -f * Time.deltaTime * enemySettings.tiltSpeed);
    }

    private void UpdateTarget()
    {
        if (target == null)
        {
            var player = GameManager.Instance.Player;
            target = player.transform;
        }

        if (Vector3.Distance(transform.position, homePosition) > enemySettings.patrolRadius)
        {
            targetPosition = homePosition;
            return;
        }
        targetPosition = target.position + target.GetComponent<Rigidbody>().velocity.magnitude * target.forward;
        //targetPosition = target.position;
    }

    private void CheckCollision()
    {
        Vector3 dir = rb.transform.forward;
        float dist;
        if (CheckDirection(dir, Vector3.zero, out dist))
        {
            float brake = 1 / (dist * .1f + 0.01f);
            desiredSpeed = 1 - brake;
            // Find different direction
            desiredDirection = this.transform.up;
            Vector3 dodgeVector = transform.forward;
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    Vector2 rotation = new Vector2(x, y);
                    Vector3 direction = transform.forward;
                    direction += x * transform.right;
                    direction += y * transform.up;
                    direction.Normalize();
                    float distance = -1f;
                    if (!CheckDirection(direction, Vector3.zero, out distance))
                    {
                        dodgeVector = direction;
                        desiredDirection = direction;
                        return;
                    }
                }
            }
            desiredDirection = -transform.forward;
        }
        else
        {
            desiredDirection = (targetPosition - transform.position).normalized;
        }
    }

    private bool CheckDirection(Vector3 worldDirection, Vector3 offset, out float distance)
    {
        Ray ray = new Ray(this.transform.position + offset, worldDirection.normalized);
        RaycastHit hit = new RaycastHit();
        if (Physics.SphereCast(ray, enemySettings.radius, out hit, enemySettings.sightDistance,
            enemySettings.collisionMask))
        {
            distance = hit.distance;
            Debug.DrawLine(transform.position, hit.point, Color.red);
            return true;
        }

        Debug.DrawLine(transform.position, transform.position + worldDirection.normalized * enemySettings.sightDistance, Color.green);
        distance = Mathf.Infinity;
        return false;
    }

    private void Move()
    {
        rb.velocity = transform.forward * (enemySettings.maxSpeed * desiredSpeed);
    }

    private float Remap(float minOld, float maxOld, float minNew, float maxNew, float value)
    {
        float outgoing =
            minNew + (maxNew - maxNew) * ((value - minOld) / (maxOld - minOld));
        return outgoing;
    }
}

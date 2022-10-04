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

    [SerializeField] private PID xAxisPID;
    [SerializeField, Range(-10, 10)] private float xAxisP, xAxisI, xAxisD;
    
    [SerializeField] private PID zAxisPID;
    [SerializeField, Range(-10, 10)] private float zAxisP, zAxisI, zAxisD;
    
    private void Update()
    {
        Debug.DrawLine(transform.position, targetPosition, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.yellow);
    }

    // Start is called before the first frame update
    void Start()
    {
        xAxisPID = new PID(xAxisP, xAxisI, xAxisD);
        rb.maxAngularVelocity = enemySettings.maxAngularVelocity;
        StartCoroutine(UpdateAll());
    }

    private void FixedUpdate()
    {
        Vector3 rotationDirection = Vector3.RotateTowards(transform.forward, desiredDirection, 360, 0.00f);
        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);

        float xAngleError = Mathf.DeltaAngle(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.x);
        float xTorqueCorrection = xAxisPID.GetOutput(xAngleError, Time.fixedDeltaTime);
        
        float zAngleError = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, targetRotation.eulerAngles.z);
        float zTorqueCorrection = zAxisPID.GetOutput(zAngleError, Time.fixedDeltaTime);
        
        rb.AddRelativeTorque(new Vector3(xTorqueCorrection, 0, zTorqueCorrection));
    }

    private void OnDisable()
    {
        StopCoroutine(UpdateAll());
    }

    IEnumerator UpdateAll()
    {
        for (;;)
        {
            UpdateTarget();
            CheckCollision();
            //Move();
            
            yield return new WaitForSeconds(updateFrequency);
        }
    }

    private void UpdateTarget()
    {
        //var player = GameManager.Instance.Player;
        //targetPosition = player.transform.position + player.GetComponent<Rigidbody>().velocity.magnitude * player.transform.forward;
        targetPosition = target.position;
    }

    private void CheckCollision()
    {
        Vector3 dir = rb.transform.forward;
        float dist;
        if (CheckDirection(dir, Vector3.zero, out dist))
        {
            desiredDirection = Vector3.Lerp(this.transform.forward, this.transform.up, 0.5f);
        }
        else
        {
            desiredDirection = (targetPosition - transform.position);
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
            return true;
        }

        distance = Mathf.Infinity;
        return false;
    }

    private void Move()
    {
        var currentAngular = rb.angularVelocity;
        var dotTilt = Vector3.Dot(transform.up, desiredDirection);
        var tilt =  Remap(-1, 1, 1, 0, dotTilt) * enemySettings.tiltSpeed;
        var dotRoll = 0;
        var roll = Vector3.Dot(-transform.right, desiredDirection) * enemySettings.rollSpeed;
        var values = new Vector3(tilt, 0, roll);
        var damped = Vector3.SmoothDamp(currentAngular, values, ref refer, updateFrequency);
        rb.angularVelocity = values;
        var acc = transform.forward * (enemySettings.acceleration * (enemySettings.maxSpeed - rb.velocity.magnitude) * Time.deltaTime);
        //rb.velocity += acc;
    }

    private float Remap(float minOld, float maxOld, float minNew, float maxNew, float value)
    {
        float outgoing =
            minNew + (maxNew - maxNew) * ((value - minOld) / (maxOld - minOld));
        return outgoing;
    }
}

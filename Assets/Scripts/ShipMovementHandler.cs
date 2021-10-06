using System;
using UnityEditor;
using UnityEngine;

public class ShipMovementHandler : MonoBehaviour
{
    [Header("Attributes")]
    //
    [SerializeField]
    private float rollSpeed = 2f;

    [SerializeField] private float yawSpeed = 0.8f;
    [SerializeField] private float pitchSpeed = 1f;

    [SerializeField] private float accelerationBackwards = 1f;
    [SerializeField] private float accelerationForwards = 2f;
    [SerializeField] private float accelerationLateral = 0.8f;
    [SerializeField] private float maxSpeed = 100f;


    [Header("Ship")]
    //
    [SerializeField]
    private GameObject shipObject;

    private InputHandler inputHandler;
    private Rigidbody shipRigidbody;

#if DEBUG
    private GUIStyle textStyle;
    private float dotX, dotY;
#endif


    // Start is called before the first frame update
    private void Start()
    {
#if DEBUG
        this.textStyle = new GUIStyle();
        this.textStyle.normal.textColor = Color.green;

#endif
        this.inputHandler = this.shipObject.GetComponent<InputHandler>();
        this.shipRigidbody = this.shipObject.GetComponent<Rigidbody>();
    }

#if DEBUG
    private void OnDrawGizmos()
    {

        if (!Application.isPlaying)
        {
            return;
        }

        if (Application.isEditor)
        {
            var position = this.shipObject.transform.position;
            Handles.Label(position, $"angV=({inputHandler.Pitch}, {inputHandler.Roll}, {inputHandler.Yaw}); V=({this.shipRigidbody.velocity.magnitude})",
                this.textStyle);
            Handles.color = Color.red;
            Handles.DrawLine(position, position + this.shipObject.transform.forward * 20f);
            Handles.color = Color.green;
            Handles.DrawLine(position, position + this.shipRigidbody.velocity);
            Handles.Label(position + this.shipObject.transform.right * 2, $"dotX:{this.dotX}", this.textStyle);
            Handles.Label(position + this.shipObject.transform.up * 2, $"dotY:{this.dotY}", this.textStyle);
        }
    }
#endif

    private void FixedUpdate()
    {
        var (pitch, roll, yaw, thrust, _) = this.inputHandler.CurrentInputState;
        this.HandleAngularVelocity(pitch, yaw, roll);
        this.HandleThrust(thrust);
    }

    private void HandleThrust(float thrust)
    {
        var forward = this.shipObject.transform.forward;
        var isLookingForwards = Vector3.Dot(forward, this.shipRigidbody.velocity) > 0;
        
        if (this.inputHandler.Braking)
        {
            this.ApplyBraking(isLookingForwards);
        }
        else
        {
            if (thrust > 0f)
            {
                var force = thrust * this.accelerationForwards;
                this.shipRigidbody.AddForce(this.shipObject.transform.forward * force);
            }
            else if (thrust < 0f)
            {
                var force = thrust * this.accelerationBackwards;
                this.shipRigidbody.AddForce(this.shipObject.transform.forward * force);
            }
        }

        if (isLookingForwards)
        {
            this.HandleStabilization();
        }
#if DEBUG
        else
        {
            this.dotX = this.dotY = float.NaN;
        }
#endif
        // Check if Speed exceeds max speed. if yes, clamp value down
        if (this.shipRigidbody.velocity.magnitude > this.maxSpeed)
        {
            this.shipRigidbody.velocity = this.shipRigidbody.velocity.normalized * this.maxSpeed;
        }
    }

    private void ApplyBraking(bool isLookingForwards)
    {
        Vector3 force;
        if (isLookingForwards)
        {
            force = -this.shipObject.transform.forward * this.accelerationBackwards;
        }
        else
        {
            force = this.shipObject.transform.forward * this.accelerationForwards;
        }
        this.shipRigidbody.AddForce(force);
    }

    private void HandleStabilization()
    {
        var vNow = this.shipRigidbody.velocity;

        // Determine which sides need to trigger their thrusters

        // Local X
        var localX = this.shipObject.transform.right;
        var dotProductCurrentDirectionXAxis = Vector3.Dot(localX, vNow);

#if DEBUG
        this.dotX = dotProductCurrentDirectionXAxis;

#endif

        if (Mathf.Abs(dotProductCurrentDirectionXAxis) > 0.05f)
        {
            this.shipRigidbody.AddForce(dotProductCurrentDirectionXAxis * this.accelerationLateral * -localX);
            if (dotProductCurrentDirectionXAxis > 0)
            {
                // Need to move left (trigger right thrusters)
                // TODO: Thruster Effect Management
            }
            else
            {
                // Need to move right (trigger left thrusters)
                // TODO: Thruster Effect Management
            }
        }

        // Local Y
        var localY = this.shipObject.transform.up;
        var dotProductCurrentDirectionYAxis = Vector3.Dot(localY, vNow);
#if DEBUG
        this.dotY = dotProductCurrentDirectionYAxis;
#endif

        if (Mathf.Abs(dotProductCurrentDirectionYAxis) > 0.1f)
        {
            this.shipRigidbody.AddForce(dotProductCurrentDirectionYAxis * this.accelerationLateral * -localY);
            if (dotProductCurrentDirectionYAxis > 0)
            {
                // Need to move left (trigger right thrusters)
                // TODO: Thruster Effect Management
            }
            else
            {
                // Need to move right (trigger left thrusters)
                // TODO: Thruster Effect Management
            }
        }
    }

    private void HandleAngularVelocity(float pitch, float yaw, float roll)
    {
        var currentWorldAngularVelocity = this.shipRigidbody.angularVelocity;
        var currentLocalAngularVelocity =
            this.shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

        var angularForce = new Vector3(-pitch * pitchSpeed, yaw * yawSpeed, -roll * rollSpeed);
        currentLocalAngularVelocity += angularForce;

        var modifiedWorldAngularVelocity = this.shipObject.transform.TransformDirection(currentLocalAngularVelocity);
        this.shipRigidbody.angularVelocity = modifiedWorldAngularVelocity;
    }
}
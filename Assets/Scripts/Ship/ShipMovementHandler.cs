using System.Numerics;
using Ship;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(InputHandler))]
public class ShipMovementHandler : MonoBehaviour
{
    [Header("Rotation Forces")]
    [SerializeField] private float rollSpeed = 2f;
    [SerializeField] private float yawSpeed = 0.8f;
    [SerializeField] private float pitchSpeed = 1f;

    [Header("Movement Forces")]
    [SerializeField] private float accelerationForwards = 2f;
    [SerializeField] private float accelerationBackwards = 1f;
    [SerializeField] public float accelerationLateral = 1f;
    [SerializeField] public float maxSpeed = 30f;
    [SerializeField] private float minBrakeSpeed = 0.02f;
    [SerializeField] public float stabilizationMultiplier = 3;

    [HideInInspector] public GameObject shipObject;
    [HideInInspector] public InputHandler inputHandler;
    [HideInInspector] public Rigidbody shipRigidbody;
    
    [HideInInspector] public bool isStrafing;
    private float _desiredSpeed = 0;

#if DEBUG
    private GUIStyle _textStyle;
    public static float DotX, DotY;
#endif

    private void Start()
    {
#if DEBUG
        this._textStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                textColor = Color.green
            }
        };
#endif
        shipObject = gameObject;
        inputHandler = shipObject.GetComponent<InputHandler>();
        shipRigidbody = shipObject.GetComponent<Rigidbody>();
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
            var position = shipObject.transform.position;
            Handles.Label(position,
                $"angV=({inputHandler.Pitch}, {inputHandler.Roll}, {inputHandler.Yaw}); V=({shipRigidbody.velocity.magnitude})",
                this._textStyle);
            Handles.color = Color.red;
            Handles.DrawLine(position, position + shipObject.transform.forward * 20f);
            Handles.color = Color.green;
            Handles.DrawLine(position, position + shipRigidbody.velocity);
            Handles.Label(position + shipObject.transform.right * 2, $"dotX:{DotX}", this._textStyle);
            Handles.Label(position + shipObject.transform.up * 2, $"dotY:{DotY}", this._textStyle);
        }
    }
#endif

    private void FixedUpdate()
    {
        var (pitch, roll, yaw, thrust, strafe, _) = inputHandler.CurrentInputState;
        this.HandleAngularVelocity(pitch, yaw, roll);
        this.HandleThrust(thrust, strafe);
        Stabilization.StabilizeShip(this);
    }

    private void HandleAngularVelocity(float pitch, float yaw, float roll)
    {
        var currentWorldAngularVelocity = shipRigidbody.angularVelocity;
        var currentLocalAngularVelocity = shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

        var angularForce = new Vector3(-pitch * pitchSpeed, yaw * yawSpeed, -roll * rollSpeed);
        currentLocalAngularVelocity += angularForce;

        var modifiedWorldAngularVelocity = shipObject.transform.TransformDirection(currentLocalAngularVelocity);
        shipRigidbody.angularVelocity = modifiedWorldAngularVelocity;
    }

    private void HandleThrust(float thrust, float strafe)
    {
        _desiredSpeed += thrust;
        if (_desiredSpeed > maxSpeed) _desiredSpeed = maxSpeed;
        else if (_desiredSpeed < 0) _desiredSpeed = 0;

        var currentSpeed = shipObject.transform.forward;
        var forwardSpeed = Vector3.Dot(currentSpeed, shipRigidbody.velocity);

        if (inputHandler.Braking)
        {
            _desiredSpeed--;
            if (_desiredSpeed < 0) _desiredSpeed = 0;
            if(forwardSpeed > 0) ApplyBraking(-accelerationBackwards);
            else ApplyBraking(accelerationForwards);
        }
        else
        {
            var thrustForce = 0f;
            if (_desiredSpeed > forwardSpeed) thrustForce = accelerationForwards;
            else if (_desiredSpeed < forwardSpeed) thrustForce = -accelerationBackwards;
            shipRigidbody.AddForce(currentSpeed * thrustForce);
        }

        isStrafing = strafe != 0;
        if (isStrafing)
        {
            var strafeForce = strafe * accelerationLateral;
            shipRigidbody.AddForce(shipObject.transform.right * strafeForce);
        }
    }

    private void ApplyBraking(float brakingForce)
    {
        if (shipRigidbody.velocity.sqrMagnitude < minBrakeSpeed) shipRigidbody.velocity = Vector3.zero;
        else shipRigidbody.AddForce(shipObject.transform.forward * brakingForce);
    }
}
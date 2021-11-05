using Ship;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(InputHandler))]
public class ShipMovementHandler : MonoBehaviour
{
    [Header("Rotation Forces")]
    [SerializeField] public float pitchSpeed = 1f;
    [SerializeField] public float rollSpeed = 2f;
    [SerializeField] public float yawSpeed = 0.8f;

    [Header("Movement Forces")]
    [SerializeField] public float accelerationForwards = 2f;
    [SerializeField] public float accelerationBackwards = 1f;
    [SerializeField] public float accelerationLateral = 1f;
    [SerializeField] public float maxSpeed = 150f;
    [SerializeField] public float maxSpeedBoost = 75f;
    [SerializeField] private float minBrakeSpeed = 0.02f;
    [SerializeField] public float stabilizationMultiplier = 3;

    [HideInInspector] public GameObject shipObject;
    [HideInInspector] public InputHandler inputHandler;
    [HideInInspector] public Rigidbody shipRigidbody;
    
    [HideInInspector] public bool isStrafing;
    [HideInInspector] public float desiredSpeed;
    [HideInInspector] public float currentSpeed;

    [HideInInspector] public string currentFlightModel = "Custom";

#if DEBUG
    private GUIStyle textStyle;
    public static float DotX, DotY;
#endif

    private void Start()
    {
#if DEBUG
        this.textStyle = new GUIStyle()
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
        
        FlightModel.StoreCustomFlightModel(this);
        FlightModel.LoadFlightModel(this,"Normal");
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
                this.textStyle);
            Handles.color = Color.red;
            Handles.DrawLine(position, position + shipObject.transform.forward * 20f);
            Handles.color = Color.green;
            Handles.DrawLine(position, position + shipRigidbody.velocity);
            Handles.Label(position + shipObject.transform.right * 2, $"dotX:{DotX}", this.textStyle);
            Handles.Label(position + shipObject.transform.up * 2, $"dotY:{DotY}", this.textStyle);
        }
    }
#endif

    private void FixedUpdate()
    {
        var (pitch, roll, yaw, thrust, strafe, _, boosting) = inputHandler.CurrentInputState;
        HandleAngularVelocity(pitch, yaw, roll, boosting);
        HandleThrust(thrust, strafe, boosting);
        currentSpeed = Stabilization.StabilizeShip(this, boosting);
        
        if (inputHandler.SwitchFlightModel)
        {
            FlightModel.NextFlightModel(this);
            inputHandler.SwitchFlightModel = false;
        }
    }

    private void HandleAngularVelocity(float pitch, float yaw, float roll, bool boosting)
    {
        var currentWorldAngularVelocity = shipRigidbody.angularVelocity;
        var currentLocalAngularVelocity = shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

        var boostMult = boosting ? 0.5f : 1f;
        var angularForce = new Vector3(-pitch * pitchSpeed * boostMult, yaw * yawSpeed * boostMult, -roll * rollSpeed);
        currentLocalAngularVelocity += angularForce;

        var modifiedWorldAngularVelocity = shipObject.transform.TransformDirection(currentLocalAngularVelocity);
        shipRigidbody.angularVelocity = modifiedWorldAngularVelocity;
    }

    private void HandleThrust(float thrust, float strafe, bool boosting)
    {
        desiredSpeed += thrust;
        if (desiredSpeed > maxSpeed) desiredSpeed = maxSpeed;
        else if (desiredSpeed < 0) desiredSpeed = 0;

        var actualSpeed = shipObject.transform.forward;
        var forwardSpeed = Vector3.Dot(actualSpeed, shipRigidbody.velocity);

        if (inputHandler.Braking)
        {
            desiredSpeed = 0;
            if(forwardSpeed > 0) ApplyBraking(-accelerationBackwards);
            else ApplyBraking(accelerationForwards);
        }
        else
        {
            var thrustForce = 0f;
            var boostedSpeed = desiredSpeed + (boosting ? maxSpeedBoost : 0);
            if (boostedSpeed > forwardSpeed) thrustForce = accelerationForwards * (boosting ? 2 : 1);
            else if (boostedSpeed < forwardSpeed) thrustForce = -accelerationBackwards;
            shipRigidbody.AddForce(actualSpeed * thrustForce);
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
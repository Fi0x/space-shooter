using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Ship
{
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
            this.shipObject = this.gameObject;
            this.inputHandler = this.shipObject.GetComponent<InputHandler>();
            this.shipRigidbody = this.shipObject.GetComponent<Rigidbody>();

            FlightModel.StoreCustomFlightModel(this);
            FlightModel.LoadFlightModel(this,"Custom");
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
                Handles.Label(position,
                    $"angV=({this.inputHandler.Pitch}, {this.inputHandler.Roll}, {this.inputHandler.Yaw}); V=({this.shipRigidbody.velocity.magnitude})",
                    this.textStyle);
                Handles.color = Color.red;
                Handles.DrawLine(position, position + this.shipObject.transform.forward * 20f);
                Handles.color = Color.green;
                Handles.DrawLine(position, position + this.shipRigidbody.velocity);
                Handles.Label(position + this.shipObject.transform.right * 2, $"dotX:{DotX}", this.textStyle);
                Handles.Label(position + this.shipObject.transform.up * 2, $"dotY:{DotY}", this.textStyle);
            }
        }
#endif

        private void FixedUpdate()
        {
            var (pitch, roll, yaw, thrust, strafe, _, boosting) = this.inputHandler.CurrentInputState;
            this.HandleAngularVelocity(pitch, yaw, roll, boosting);
            this.HandleThrust(thrust, strafe, boosting);
            this.currentSpeed = Stabilization.StabilizeShip(this, boosting);

            if (this.inputHandler.SwitchFlightModel)
            {
                FlightModel.NextFlightModel(this);
                this.inputHandler.SwitchFlightModel = false;
            }
        }

        private void HandleAngularVelocity(float pitch, float yaw, float roll, bool boosting)
        {
            var currentWorldAngularVelocity = this.shipRigidbody.angularVelocity;
            var currentLocalAngularVelocity = this.shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

            var boostMult = boosting ? 0.5f : 1f;
            var angularForce = new Vector3(-pitch * this.pitchSpeed * boostMult, yaw * this.yawSpeed * boostMult, -roll * this.rollSpeed);
            currentLocalAngularVelocity += angularForce;

            var modifiedWorldAngularVelocity = this.shipObject.transform.TransformDirection(currentLocalAngularVelocity);
            this.shipRigidbody.angularVelocity = modifiedWorldAngularVelocity;
        }

        private void HandleThrust(float thrust, float strafe, bool boosting)
        {
            this.desiredSpeed += thrust;
            if (this.desiredSpeed > this.maxSpeed)
                this.desiredSpeed = this.maxSpeed;
            else if (this.desiredSpeed < 0) this.desiredSpeed = 0;

            var actualSpeed = this.shipObject.transform.forward;
            var forwardSpeed = Vector3.Dot(actualSpeed, this.shipRigidbody.velocity);

            if (this.inputHandler.Braking)
            {
                this.desiredSpeed = 0;
                if(forwardSpeed > 0)
                    this.ApplyBraking(-this.accelerationBackwards);
                else
                    this.ApplyBraking(this.accelerationForwards);
            }
            else
            {
                var thrustForce = 0f;
                var boostedSpeed = this.desiredSpeed + (boosting ? this.maxSpeedBoost : 0);
                if (boostedSpeed > forwardSpeed) thrustForce = this.accelerationForwards * (boosting ? 2 : 1);
                else if (boostedSpeed < forwardSpeed) thrustForce = -this.accelerationBackwards;
                this.shipRigidbody.AddForce(actualSpeed * thrustForce);
            }

            this.isStrafing = strafe != 0;
            if (this.isStrafing)
            {
                var strafeForce = strafe * this.accelerationLateral;
                this.shipRigidbody.AddForce(this.shipObject.transform.right * strafeForce);
            }
        }

        private void ApplyBraking(float brakingForce)
        {
            if (this.shipRigidbody.velocity.sqrMagnitude < this.minBrakeSpeed)
                this.shipRigidbody.velocity = Vector3.zero;
            else
                this.shipRigidbody.AddForce(this.shipObject.transform.forward * brakingForce);
        }
    }
}
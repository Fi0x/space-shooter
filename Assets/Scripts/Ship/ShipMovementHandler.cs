using System;
using Manager;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Ship
{
    [RequireComponent(typeof(InputHandler))] [Obsolete]
    public class sShipMovementHandler : MonoBehaviour
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
        [SerializeField] private float speedMatchDeadZone = 0.01f;

        [HideInInspector] public GameObject shipObject;
        [HideInInspector] public InputHandler inputHandler;
        [HideInInspector] public Rigidbody shipRigidbody;

        [SerializeField, ReadOnlyInspector] public bool isStrafing;
        [SerializeField, ReadOnlyInspector] public float desiredSpeed;
        [SerializeField, ReadOnlyInspector] public float currentSpeed;

        [SerializeField, ReadOnlyInspector] public string currentFlightModel = "Custom";

        public static float TotalMaxSpeed { get; set; }

        /// <summary>
        /// This Event in invoked <b>after</b> forces have been applied onto the Ship's Rigidbody.
        /// The Camera uses this event to modify its position after forces.
        /// </summary>
        public event Action ForcesAppliedEvent;

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
            FlightModel.LoadFlightModel(this,"Hyper");

            TotalMaxSpeed = this.maxSpeed + this.maxSpeedBoost;
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
            // Necessary to allow full-stop
            if(this.shipRigidbody.velocity.sqrMagnitude < this.minBrakeSpeed) this.shipRigidbody.velocity = Vector3.zero;
            
            var input = this.inputHandler.CurrentInputState;
            this.HandleAngularVelocity(input.Pitch, input.Yaw, input.Roll);
            this.HandleThrust(input.Thrust, input.Strafe);
            this.currentSpeed = Stabilization.StabilizeShip(this);

            this.ForcesAppliedEvent?.Invoke();

            if (this.inputHandler.SwitchFlightModel)
            {
                FlightModel.NextFlightModel(this);
                this.inputHandler.SwitchFlightModel = false;
            }
        }

        private void HandleAngularVelocity(float pitch, float yaw, float roll)
        {
            var currentWorldAngularVelocity = this.shipRigidbody.angularVelocity;
            var currentLocalAngularVelocity = this.shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

            var mouseMultiplier = (this.inputHandler.IsBoosting ? 0.5f : 1f) * InputManager.MouseSensitivity;
            var pitchForce = -pitch * this.pitchSpeed * mouseMultiplier;
            var yawForce = yaw * this.yawSpeed * mouseMultiplier;
            var rollForce = -roll * this.rollSpeed;
            
            var angularForce = new Vector3(pitchForce, yawForce, rollForce);
            currentLocalAngularVelocity += angularForce;

            var modifiedWorldAngularVelocity = this.shipObject.transform.TransformDirection(currentLocalAngularVelocity);
            this.shipRigidbody.angularVelocity = modifiedWorldAngularVelocity;
        }

        /// <summary>
        /// Handles any types of Ship Movement before stabilization
        /// </summary>
        /// <param name="dThrust">Change in Thrust. As of now this is either +1 or -1 and affects the "speed target"</param>
        /// <param name="strafeX">Change Left/Right Thrust. As of now, this is either +1 or -1.</param>
        /// <param name="isBoosting">Is Player holding down Boost Button</param>
        private void HandleThrust(float dThrust, float strafeX)
        {
            if (this.inputHandler.Braking)
            {
                this.desiredSpeed = 0;
            }
            else
            {
                this.desiredSpeed += dThrust * (this.maxSpeed + this.maxSpeedBoost) * 0.01f;
                
                if (this.desiredSpeed > this.maxSpeed)
                    this.desiredSpeed = this.maxSpeed;
                else if (this.desiredSpeed < -this.maxSpeed)
                    this.desiredSpeed = -this.maxSpeed;
            }

            var currentForwardDirection = this.shipObject.transform.forward;

            // See https://i.imgur.com/p70W4s6.png
            var currentEffectiveForwardSpeed = Vector3.Dot(currentForwardDirection, this.shipRigidbody.velocity);

            var targetSpeed = this.desiredSpeed;
            if (this.inputHandler.IsBoosting)
            {
                targetSpeed = this.maxSpeed + this.maxSpeedBoost;
            }

            // Check small deviations around target speed ("DeadZone")

            if (currentEffectiveForwardSpeed + this.speedMatchDeadZone > targetSpeed &&
                currentEffectiveForwardSpeed - this.speedMatchDeadZone < targetSpeed)
            {
                // Inside DeadZone. Check if currently speed is zero. If this is the case, set the velocity to 0
                if (targetSpeed == 0f)
                {
                    // Set forward velocity to 0
                    this.shipRigidbody.velocity = Vector3.Scale(new Vector3(1, 1, 0), this.shipRigidbody.velocity);
                }
            }
            else if (targetSpeed > currentEffectiveForwardSpeed)
            {
                var thrustForce = this.accelerationForwards * (this.inputHandler.IsBoosting ? 2 : 1);
                this.shipRigidbody.AddForce(currentForwardDirection * thrustForce);

                var newEffectiveForwardSpeed = Vector3.Dot(currentForwardDirection, this.shipRigidbody.velocity);
                if (targetSpeed < newEffectiveForwardSpeed)
                {
                    // "overshot" target when thrusting.
                    // clamp value
                    var shipRigidbodyVelocity = this.shipRigidbody.velocity;
                    this.shipRigidbody.velocity = shipRigidbodyVelocity.normalized *
                                      (shipRigidbodyVelocity.magnitude / newEffectiveForwardSpeed);
                }

            }
            else if (targetSpeed < currentEffectiveForwardSpeed)
            {
                var thrustForce = -this.accelerationBackwards;
                this.shipRigidbody.AddForce(currentForwardDirection * thrustForce);

                var newEffectiveForwardSpeed = Vector3.Dot(currentForwardDirection, this.shipRigidbody.velocity);
                if (targetSpeed > newEffectiveForwardSpeed)
                {
                    // "overshot" target when thrusting.
                    // clamp value
                    var shipRigidbodyVelocity = this.shipRigidbody.velocity;
                    this.shipRigidbody.velocity = shipRigidbodyVelocity.normalized *
                                                  (shipRigidbodyVelocity.magnitude / newEffectiveForwardSpeed);
                }
            }

            this.HandleStrafing(strafeX);
        }

        private void HandleStrafing(float strafeX)
        {
            this.isStrafing = strafeX != 0;
            if (this.isStrafing)
            {
                var strafeForce = strafeX * this.accelerationLateral;
                this.shipRigidbody.AddForce(this.shipObject.transform.right * strafeForce);
            }
        }
    }
}
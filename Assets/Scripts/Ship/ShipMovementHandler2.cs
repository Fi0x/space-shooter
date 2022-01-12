using System;
using Manager;
using UnityEngine;

namespace Ship
{
    public class ShipMovementHandler2 : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private ShipMovementHandler2Settings settings;

        [Header("Debug Data")] [SerializeField, ReadOnlyInspector]
        private float totalMaxSpeed;


        private GameObject shipObject;
        private Rigidbody shipRB;
        private InputHandler inputHandler;
        private float desiredSpeed;
        public Rigidbody ShipRB => this.shipRB;
        public InputHandler InputHandler => this.inputHandler;

        public ShipMovementHandler2Settings Settings => this.settings;

        public float TotalMaxSpeed => this.totalMaxSpeed;
        public event Action<float, float> DesiredSpeedChangedEvent;

        /// <summary>
        /// This Event in invoked <b>after</b> forces have been applied onto the Ship's Rigidbody.
        /// The Camera uses this event to modify its position after forces.
        /// </summary>
        public event Action ForcesAppliedEvent;

        private void OnEnable()
        {
            this.settings.SettingsChangedEvent += this.HandleSettingsUpdatedEvent;
        }

        private void OnDisable()
        {
            this.settings.SettingsChangedEvent -= this.HandleSettingsUpdatedEvent;
        }

        private void Start()
        {
            this.shipObject = this.gameObject;
            this.shipRB = this.shipObject.GetComponent<Rigidbody>();
            this.inputHandler = this.shipObject.GetComponent<InputHandler>();

            // TODO: Flight Model Loading
            FlightModel.StoreCustomFlightModel(this.settings);
            FlightModel.LoadFlightModel(this.settings, "Custom");
            this.totalMaxSpeed = this.settings.MaxSpeed + this.settings.MaxSpeedBoost;
        }

        private void FixedUpdate()
        {
            var input = this.inputHandler.CurrentInputState;
            this.ApplyInputChanges(input);

            this.HandleAngularVelocity(input);
            this.ModifyShipVector(input);

        }

        private void ApplyInputChanges(InputHandler.InputState input)
        {
            var oldDesiredSpeed = this.desiredSpeed;
            if (input.Braking)
            {
                this.desiredSpeed = 0f;
            }
            else
            {
                this.desiredSpeed += input.Thrust * (this.settings.MaxSpeed + this.settings.MaxSpeedBoost) * 0.01f;

                if (this.desiredSpeed > this.settings.MaxSpeed)
                {
                    // Clamp if at max speed
                    this.desiredSpeed = this.settings.MaxSpeed;
                }
                else if (this.desiredSpeed < -this.settings.MaxSpeed)
                {
                    // Clamp if at max reverse speed
                    this.desiredSpeed = -this.settings.MaxSpeed;
                }
            }
            if (Math.Abs(this.desiredSpeed - oldDesiredSpeed) > 0.1)
            {
                this.DesiredSpeedChangedEvent?.Invoke(this.desiredSpeed, this.TotalMaxSpeed);
            }
        }

        private void ModifyShipVector(InputHandler.InputState input)
        {
            var shipForward = this.shipObject.transform.forward;
            var targetVector = shipForward * this.desiredSpeed;
            if (input.Strafe != 0.0f)
            {
                targetVector += Vector3.right * input.Strafe * 10f;
            }

            var currentDirection = this.shipRB.velocity;
            var currentDirectionLocalSpace = this.transform.TransformDirection(currentDirection);
            var targetVectorLocalSpace = this.transform.TransformDirection(targetVector);

            var differenceCurrentDirectionToTargetLocalSpace = targetVectorLocalSpace - currentDirectionLocalSpace;

            this.HandleLateralThrust(
                differenceCurrentDirectionToTargetLocalSpace, currentDirectionLocalSpace, targetVectorLocalSpace);

            this.HandleMainThrust(differenceCurrentDirectionToTargetLocalSpace, targetVectorLocalSpace);



            /*
            // Apply X Correction
            if (targetVectorLocalSpace.x < currentDirectionLocalSpace.x)
            {
                this.shipRB.AddRelativeForce(Vector3.right * this.settings.AccelerationLateral * Time.fixedDeltaTime);
                if (targetVectorLocalSpace.x > this.transform.TransformDirection(this.shipRB.velocity).x)
                {
                    // Overflew target – clamp to target
                    var currentDirectionAfterForceApplicationLocalSpace =
                        this.transform.TransformDirection(this.shipRB.velocity);
                    this.shipRB.velocity = this.transform.InverseTransformDirection(new Vector3(targetVector.x,  currentDirectionAfterForceApplicationLocalSpace.y));
                }
            }
            else if (targetVectorLocalSpace.x < currentDirectionLocalSpace.x)
            {
                this.shipRB.AddRelativeForce(Vector3.left * this.settings.AccelerationLateral * Time.fixedDeltaTime);
                if (targetVectorLocalSpace.x < this.transform.TransformDirection(this.shipRB.velocity).x)
                {
                    // Overflew target – clamp to target
                    var currentDirectionAfterForceApplicationLocalSpace =
                        this.transform.TransformDirection(this.shipRB.velocity);
                    this.shipRB.velocity = this.transform.InverseTransformDirection(new Vector3(targetVector.x,
                        currentDirectionAfterForceApplicationLocalSpace.y));
                }
            }
            // Apply Y Correction
            if (targetVectorLocalSpace.y > currentDirectionLocalSpace.y)
            {
                this.shipRB.AddRelativeForce(Vector3.up * this.settings.AccelerationLateral * Time.fixedDeltaTime);
                if (targetVectorLocalSpace.y < this.transform.TransformDirection(this.shipRB.velocity).y)
                {
                    // Overflew target – clamp to target
                    var currentDirectionAfterForceApplicationLocalSpace =
                        this.transform.TransformDirection(this.shipRB.velocity);
                    this.shipRB.velocity = this.transform.InverseTransformDirection(new Vector3(currentDirectionAfterForceApplicationLocalSpace.x, targetVector.y));
                }
            }
            else if (targetVectorLocalSpace.y < currentDirectionLocalSpace.y)
            {
                this.shipRB.AddRelativeForce(Vector3.down * this.settings.AccelerationLateral * Time.fixedDeltaTime);
                if (targetVectorLocalSpace.y > this.transform.TransformDirection(this.shipRB.velocity).y)
                {
                    // Overflew target – clamp to target
                    var currentDirectionAfterForceApplicationLocalSpace =
                        this.transform.TransformDirection(this.shipRB.velocity);
                    this.shipRB.velocity = this.transform.InverseTransformDirection(new Vector3(currentDirectionAfterForceApplicationLocalSpace.x, targetVector.y));
                }
            }
            */
        }

        private void HandleSettingsUpdatedEvent(ShipMovementHandler2Settings settings)
        {
            this.totalMaxSpeed = settings.MaxSpeed + settings.MaxSpeedBoost;
            this.SettingsUpdatedEvent?.Invoke(settings);
        }

        private void HandleMainThrust(Vector3 differenceCurrentDirectionToTargetLocalSpace, Vector3 targetVectorLocalSpace)
        {
            if (differenceCurrentDirectionToTargetLocalSpace.z != 0)
            {
                var currentVelocityLocal = this.transform.InverseTransformDirection(this.shipRB.velocity);
                if (differenceCurrentDirectionToTargetLocalSpace.z > 0)
                {
                    this.shipRB.AddRelativeForce(Vector3.forward * this.settings.AccelerationForwards * Time.fixedDeltaTime);
                    var velocityAfterForceLocal = this.transform.InverseTransformDirection(this.shipRB.velocity);
                    if (!this.IsValueInBetween(currentVelocityLocal.z, targetVectorLocalSpace.z,
                        velocityAfterForceLocal.z))
                    {
                        var newForceLocal = velocityAfterForceLocal;
                        newForceLocal.z = targetVectorLocalSpace.z;
                        this.shipRB.velocity = this.transform.TransformDirection(newForceLocal);
                    }
                }
                else
                {
                    this.shipRB.AddRelativeForce(Vector3.back * this.settings.AccelerationBackwards * Time.fixedDeltaTime);
                    var velocityAfterForceLocal = this.transform.InverseTransformDirection(this.shipRB.velocity);
                    if (!this.IsValueInBetween(currentVelocityLocal.z, targetVectorLocalSpace.z,
                        velocityAfterForceLocal.z))
                    {
                        var newForceLocal = velocityAfterForceLocal;
                        newForceLocal.z = targetVectorLocalSpace.z;
                        this.shipRB.velocity = this.transform.TransformDirection(newForceLocal);
                    }
                }
            }
        }

        private void HandleLateralThrust(Vector3 differenceCurrentDirectionToTargetLocalSpace, Vector3 currentDirectionLocalSpace, Vector3 targetVectorLocalSpace)
        {
            if (differenceCurrentDirectionToTargetLocalSpace.x != 0 ||
                differenceCurrentDirectionToTargetLocalSpace.y != 0)
            {
                var lateralForceToApplyLocalSpace = Vector3.Scale(Vector3.one - Vector3.forward,
                    -differenceCurrentDirectionToTargetLocalSpace).normalized * this.settings.AccelerationLateral;
                this.shipRB.AddRelativeForce(lateralForceToApplyLocalSpace);
                // Check for Overflow
                var newCurrentDirectionLocalSpace = this.transform.InverseTransformDirection(this.shipRB.velocity);

                var needClamp = false;

                if (differenceCurrentDirectionToTargetLocalSpace.x != 0)
                {
                    // Check if x is outside currentDir and target. if that's the case, clamp.
                    var xOutsideBounds = !this.IsValueInBetween(currentDirectionLocalSpace.x,
                        newCurrentDirectionLocalSpace.x, differenceCurrentDirectionToTargetLocalSpace.x);
                    if (xOutsideBounds)
                    {
                        needClamp = true;
                    }
                }
                else
                {
                    // Just as a fallback if x is zero. This is because we need to be able to check if an overflow
                    // has occurred. This cannot be done if the change only occured on the Y Axis.
                    var yOutsideBounds = !this.IsValueInBetween(currentDirectionLocalSpace.y,
                        newCurrentDirectionLocalSpace.y, differenceCurrentDirectionToTargetLocalSpace.y);
                    if (yOutsideBounds)
                    {
                        needClamp = true;
                    }
                }

                if (needClamp)
                {
                    newCurrentDirectionLocalSpace.x = targetVectorLocalSpace.x;
                    newCurrentDirectionLocalSpace.y = targetVectorLocalSpace.y;
                    var newCurrentDirectionWorldSpace =
                        this.transform.TransformDirection(newCurrentDirectionLocalSpace);
                    this.shipRB.velocity = newCurrentDirectionWorldSpace;
                }
            }

        }

        private bool IsValueInBetween(float bound1, float bound2, float valueToCheck, bool boundsInclusive = false)
        {
            float lower, upper;
            if (bound1 < bound2)
            {
                lower = bound1;
                upper = bound2;
            }
            else
            {
                lower = bound2;
                upper = bound1;
            }

            if (boundsInclusive)
            {
                return valueToCheck >= lower && valueToCheck <= upper;
            }
            else
            {
                return valueToCheck > lower && valueToCheck < upper;
            }
        }

        private void HandleAngularVelocity(InputHandler.InputState input)
        {
            var currentWorldAngularVelocity = this.shipRB.angularVelocity;
            var currentLocalAngularVelocity = this.shipObject.transform.InverseTransformDirection(currentWorldAngularVelocity);

            var mouseMultiplier = (this.inputHandler.IsBoosting ? 0.5f : 1f) * InputManager.MouseSensitivity;
            var pitchForce = -input.Pitch * this.settings.PitchSpeed * mouseMultiplier;
            var yawForce = input.Yaw * this.settings.YawSpeed * mouseMultiplier;
            var rollForce = -input.Roll * this.settings.RollSpeed;

            var angularForce = new Vector3(pitchForce, yawForce, rollForce);
            currentLocalAngularVelocity += angularForce;

            var modifiedWorldAngularVelocity = this.shipObject.transform.TransformDirection(currentLocalAngularVelocity);
            this.shipRB.angularVelocity = modifiedWorldAngularVelocity;
        }

        public void SetNewTargetSpeed(int newSpeed)
        {
            this.desiredSpeed = newSpeed;
        }

        public event Action<ShipMovementHandler2Settings> SettingsUpdatedEvent;
    }
}
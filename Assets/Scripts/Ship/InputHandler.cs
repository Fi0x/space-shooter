#define FIX_POSITION

using Manager;
using UnityEngine;

namespace Ship
{
    public class InputHandler : MonoBehaviour
    {
        private enum HorizontalAxisMode
        {
            Yaw,
            Roll
        }

        private enum VerticalAxisMode
        {
            Pitch,
            PitchInverted,
            Thrust
        }

        [Header("Rotation Controls")] [SerializeField]
        private HorizontalAxisMode xAxisMouseMode;

        [SerializeField] private VerticalAxisMode yAxisMouseMode;
        [SerializeField] private KeyCode rollLeftKey;
        [SerializeField] private KeyCode rollRightKey;

        [Header("Movement Controls")] [SerializeField]
        private KeyCode accelerateKey;

        [SerializeField] private KeyCode decelerateKey;
        [SerializeField] private KeyCode strafeLeftKey;
        [SerializeField] private KeyCode strafeRightKey;
        [SerializeField] private KeyCode brakingKey;
        [SerializeField] private KeyCode boosterKey;
        [SerializeField] private KeyCode flightModeSwitchKey;
        [SerializeField] private bool debugForceShootingTrue;
        [SerializeField] private float thrustAdjustment = 1;

        [Header("Menu Controls")] [SerializeField]
        private KeyCode pauseKey;

        public (float pitch, float roll, float yaw, float thrust, float strafe, bool braking, bool boosting) CurrentInputState { get; private set; } = (0f, 0f, 0f, 0f, 0f, false, false);

        // Getters
        public float Roll => this.CurrentInputState.roll;
        public float Pitch => this.CurrentInputState.pitch;
        public float Yaw => this.CurrentInputState.yaw;

        public bool IsShooting { get; private set; }

        public bool Braking => this.CurrentInputState.braking;

        public bool SwitchFlightModel { get; set; }

        private void Start()
        {
#if FIX_POSITION
            Cursor.lockState = CursorLockMode.Locked;
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(this.pauseKey)) GameManager.ChangePauseState();
            if(GameManager.IsGamePaused) return;
            var mouseAxes = (x: Input.GetAxis("Mouse X"), y: Input.GetAxis("Mouse Y"));
            this.CurrentInputState = this.CalculateAppliedMovement(mouseAxes);
            this.IsShooting = Input.GetMouseButton(0) || this.debugForceShootingTrue;
        }

        private (float pitch, float roll, float yaw, float thrust, float strafe, bool braking, bool boosting) CalculateAppliedMovement((float x, float y) mouseAxes)
        {
            float pitch = 0, roll = 0, yaw = 0, thrust = 0, strafe = 0;

            switch (this.xAxisMouseMode)
            {
                case HorizontalAxisMode.Roll:
                    roll = mouseAxes.x;
                    break;
                case HorizontalAxisMode.Yaw:
                    yaw = mouseAxes.x;
                    break;
            }

            switch (this.yAxisMouseMode)
            {
                case VerticalAxisMode.Pitch:
                    pitch = mouseAxes.y;
                    break;
                case VerticalAxisMode.PitchInverted:
                    pitch = -mouseAxes.y;
                    break;
                case VerticalAxisMode.Thrust:
                    thrust = mouseAxes.y;
                    break;
            }

            if (Input.GetKey(this.accelerateKey) && !Input.GetKey(this.decelerateKey)) thrust += this.thrustAdjustment;
            if (Input.GetKey(this.decelerateKey) && !Input.GetKey(this.accelerateKey)) thrust -= this.thrustAdjustment;

            if (Input.GetKey(this.strafeLeftKey) && !Input.GetKey(this.strafeRightKey)) strafe--;
            if (Input.GetKey(this.strafeRightKey) && !Input.GetKey(this.strafeLeftKey)) strafe++;

            if (Input.GetKey(this.rollLeftKey) && !Input.GetKey(this.rollRightKey)) roll = -1;
            if (Input.GetKey(this.rollRightKey) && !Input.GetKey(this.rollLeftKey)) roll = 1;

            var isBraking = Input.GetKey(this.brakingKey);
            var isBoosting = Input.GetKey(this.boosterKey);

            if (Input.GetKeyDown(this.flightModeSwitchKey)) this.SwitchFlightModel = true;

            return (pitch, roll, yaw, thrust, strafe, isBraking, isBoosting);
        }
    }
}
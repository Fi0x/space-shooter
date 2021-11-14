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

        [Header("Movement Controls")]
        [SerializeField] private bool debugForceShootingTrue;

        public (float pitch, float roll, float yaw, float thrust, float strafe, bool braking, bool boosting) CurrentInputState { get; private set; } = (0f, 0f, 0f, 0f, 0f, false, false);

        // Getters
        public float Roll => this.CurrentInputState.roll;
        public float Pitch => this.CurrentInputState.pitch;
        public float Yaw => this.CurrentInputState.yaw;

        public bool IsShooting { get; private set; }
        public bool IsBoosting => this.CurrentInputState.boosting;

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
            if(InputManager.WaitingForKeyInput) return;
            if (Input.GetKeyDown(InputManager.PauseKey)) GameManager.ChangePauseState();
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

            if (Input.GetKey(InputManager.AccelerateKey) && !Input.GetKey(InputManager.DecelerateKey)) thrust ++;
            if (Input.GetKey(InputManager.DecelerateKey) && !Input.GetKey(InputManager.AccelerateKey)) thrust --;

            if (Input.GetKey(InputManager.StrafeLeftKey) && !Input.GetKey(InputManager.StrafeRightKey)) strafe--;
            if (Input.GetKey(InputManager.StrafeRightKey) && !Input.GetKey(InputManager.StrafeLeftKey)) strafe++;

            if (Input.GetKey(InputManager.RollLeftKey) && !Input.GetKey(InputManager.RollRightKey)) roll = -1;
            if (Input.GetKey(InputManager.RollRightKey) && !Input.GetKey(InputManager.RollLeftKey)) roll = 1;

            var isBraking = Input.GetKey(InputManager.BrakingKey);
            var isBoosting = Input.GetKey(InputManager.BoostKey);

            if (Input.GetKeyDown(InputManager.FlightModeSwitchKey)) this.SwitchFlightModel = true;

            return (pitch, roll, yaw, thrust, strafe, isBraking, isBoosting);
        }
    }
}
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

        public InputState CurrentInputState { get; private set; } = InputState.None;

        public class InputState
        {
            public float Pitch { get; private set; }
            public float Roll { get; private set; }
            public float Yaw { get; private set; }
            public float Thrust { get; private set; }
            public float Strafe { get; private set; }
            public bool Braking { get; private set; }
            public bool Boosting { get; private set; }

            public bool Shooting { get; private set; }

            internal static InputState None => new InputState(0f, 0f, 0f, 0f, 0f, false, false, false);

            private InputState(float pitch, float roll, float yaw, float thrust, float strafe, bool braking,
                bool boosting, bool isShooting)
            {
                this.PushNewState(pitch, roll, yaw, thrust, strafe, braking, boosting, isShooting);
            }
            //(float pitch, float roll, float yaw, float thrust, float strafe, bool braking, bool boosting)
            public void PushNewState(float pitch, float roll, float yaw, float thrust, float strafe, bool isBraking, bool isBoosting, bool isShooting)
            {
                this.Pitch = pitch;
                this.Roll = roll;
                this.Yaw = yaw;
                this.Thrust = thrust;
                this.Strafe = strafe;
                this.Braking = isBraking;
                this.Boosting = isBoosting;
                this.Shooting = isShooting;
            }


        }

        // Getters
        public float Roll => this.CurrentInputState.Roll;
        public float Pitch => this.CurrentInputState.Pitch;
        public float Yaw => this.CurrentInputState.Yaw;

        public bool IsBoosting => this.CurrentInputState.Boosting;

        public bool Braking => this.CurrentInputState.Braking;

        public bool SwitchFlightModel { get; set; }

        private void Update()
        {
            if(InputManager.WaitingForKeyInput) return;
            if (Input.GetKeyDown(InputManager.PauseKey)) GameManager.ChangePauseState();
            if(GameManager.IsGamePaused) return;
            var mouseAxes = (x: Input.GetAxis("Mouse X"), y: Input.GetAxis("Mouse Y"));
            this.CalculateNewInputValues(mouseAxes);
            
        }

        private void CalculateNewInputValues((float x, float y) mouseAxes)
        {
            float pitch = 0, roll = 0, yaw = 0, thrust = 0, strafe = 0;

            (roll, yaw, pitch, thrust) = this.HandleMouseAxes(mouseAxes, roll, yaw, pitch, thrust);

            if (Input.GetKey(InputManager.AccelerateKey) && !Input.GetKey(InputManager.DecelerateKey)) thrust ++;
            if (Input.GetKey(InputManager.DecelerateKey) && !Input.GetKey(InputManager.AccelerateKey)) thrust --;

            if (Input.GetKey(InputManager.StrafeLeftKey) && !Input.GetKey(InputManager.StrafeRightKey)) strafe--;
            if (Input.GetKey(InputManager.StrafeRightKey) && !Input.GetKey(InputManager.StrafeLeftKey)) strafe++;

            if (Input.GetKey(InputManager.RollLeftKey) && !Input.GetKey(InputManager.RollRightKey)) roll = -1;
            if (Input.GetKey(InputManager.RollRightKey) && !Input.GetKey(InputManager.RollLeftKey)) roll = 1;

            var isBraking = Input.GetKey(InputManager.BrakingKey);
            var isBoosting = Input.GetKey(InputManager.BoostKey);

            if (Input.GetKeyDown(InputManager.FlightModeSwitchKey)) this.SwitchFlightModel = true;

            var isShooting = Input.GetMouseButton(0) || this.debugForceShootingTrue;

            this.CurrentInputState.PushNewState(pitch, roll, yaw, thrust, strafe, isBraking, isBoosting, isShooting);
        }

        private (float roll, float yaw, float pitch, float thrust) HandleMouseAxes((float x, float y) mouseAxes, float roll,
            float yaw, float pitch, float thrust)
        {
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

            return (roll, yaw, pitch, thrust);
        }
    }
}
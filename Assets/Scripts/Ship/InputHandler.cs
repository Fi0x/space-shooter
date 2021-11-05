#define FIX_POSITION

using UnityEngine;

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

    [Header("Rotation Controls")]
    [SerializeField] private HorizontalAxisMode xAxisMouseMode;
    [SerializeField] private VerticalAxisMode yAxisMouseMode;
    [SerializeField] private KeyCode rollLeftKey;
    [SerializeField] private KeyCode rollRightKey;

    [Header("Movement Controls")]
    [SerializeField] private KeyCode accelerateKey;
    [SerializeField] private KeyCode decelerateKey;
    [SerializeField] private KeyCode strafeLeftKey;
    [SerializeField] private KeyCode strafeRightKey;
    [SerializeField] private KeyCode brakingKey;
    [SerializeField] private KeyCode boosterKey;
    [SerializeField] private KeyCode flightModeSwitchKey;
    [SerializeField] private bool debugForceShootingTrue;
    [SerializeField] private float thrustAdjustment = 1;


    public (float pitch, float roll, float yaw, float thrust, float strafe, bool braking, bool boosting, bool nextFlightMode) CurrentInputState { get; private set; } = (0f, 0f, 0f, 0f, 0f, false, false, false);

    // Getters
    public float Roll => this.CurrentInputState.roll;
    public float Pitch => this.CurrentInputState.pitch;
    public float Yaw => this.CurrentInputState.yaw;
    public float Thrust => this.CurrentInputState.thrust;

    public bool IsShooting { get; private set; }

    public bool Braking => this.CurrentInputState.braking;

    public bool Boosting => this.CurrentInputState.boosting;

    private void Start()
    {
#if FIX_POSITION
        Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    // Update is called once per frame
    private void Update()
    {
        var mouseAxes = (x: Input.GetAxis("Mouse X"), y: Input.GetAxis("Mouse Y"));
        this.CurrentInputState = this.CalculateAppliedMovement(mouseAxes);
        this.IsShooting = Input.GetMouseButton(0) || this.debugForceShootingTrue;
    }

    private (float pitch, float roll, float yaw, float thrust, float strafe, bool braking, bool boosting, bool nextFlightMode) CalculateAppliedMovement((float x, float y) mouseAxes)
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

        if (Input.GetKey(accelerateKey) && !Input.GetKey(decelerateKey)) thrust += thrustAdjustment;
        if (Input.GetKey(decelerateKey) && !Input.GetKey(accelerateKey)) thrust -= thrustAdjustment;

        if (Input.GetKey(strafeLeftKey) && !Input.GetKey(strafeRightKey)) strafe--;
        if (Input.GetKey(strafeRightKey) && !Input.GetKey(strafeLeftKey)) strafe++;

        if (Input.GetKey(rollLeftKey) && !Input.GetKey(rollRightKey)) roll = -1;
        if (Input.GetKey(rollRightKey) && !Input.GetKey(rollLeftKey)) roll = 1;

        var isBraking = Input.GetKey(brakingKey);
        var isBoosting = Input.GetKey(boosterKey);
        var switchFlightMode = Input.GetKeyDown(flightModeSwitchKey);

        return (pitch, roll, yaw, thrust, strafe, isBraking, isBoosting, switchFlightMode);
    }
}
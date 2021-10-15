#define FIX_POSITION

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public enum HorizontalAxisMode
    {
        Yaw,
        Roll
    }

    public enum VerticalAxisMode
    {
        Pitch,
        PitchInverted,
        Thrust
    }

    [Header("Rotation Controls")]
    [SerializeField] private HorizontalAxisMode xAxisMouseMode;
    [SerializeField] private VerticalAxisMode yAxisMouseMode;
    [SerializeField] private KeyCode rollLeftKey; //TODO: Use for roll
    [SerializeField] private KeyCode rollRightKey; //TODO: Use for roll

    [Header("Movement Controls")]
    [SerializeField] private KeyCode accelerateKey;
    [SerializeField] private KeyCode decelerateKey;
    [SerializeField] private KeyCode straveLeftKey;
    [SerializeField] private KeyCode straveRightKey;
    [SerializeField] private KeyCode brakingKey;


    public (float pitch, float roll, float yaw, float thrust, float strave, bool braking) CurrentInputState { get; private set; } = (0f, 0f, 0f, 0f, 0f, false);

    // Getters
    public float Roll => this.CurrentInputState.roll;
    public float Pitch => this.CurrentInputState.pitch;
    public float Yaw => this.CurrentInputState.yaw;
    public float Thrust => this.CurrentInputState.thrust;

    public bool Braking => this.CurrentInputState.braking;

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
    }

    private (float pitch, float roll, float yaw, float thrust, float strave, bool braking) CalculateAppliedMovement((float x, float y) mouseAxes)
    {
        float pitch = 0, roll = 0, yaw = 0, thrust = 0, strave = 0;

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

        if (Input.GetKeyDown("W")) thrust++;
        if (Input.GetKeyDown("S")) thrust--;

        if (Input.GetKeyDown("A")) strave--;
        if (Input.GetKeyDown("D")) strave++;

        var isBraking = Input.GetKey(this.brakingKey);

        return (pitch, roll, yaw, thrust, strave, isBraking);

    }
}
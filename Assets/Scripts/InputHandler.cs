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

    [Header("Input Settings")]
    //
    [SerializeField]
    private HorizontalAxisMode xAxisMode;

    [SerializeField] private VerticalAxisMode yAxisMode;

    [SerializeField] private HorizontalAxisMode xAxisMouseMode;

    [SerializeField] private VerticalAxisMode yAxisMouseMode;

    [SerializeField] private KeyCode brakingKey;


    public (float pitch, float roll, float yaw, float thrust, bool braking) CurrentInputState { get; private set; } = (0f, 0f, 0f, 0f, false);

    // Getters
    public float Roll => this.CurrentInputState.roll;
    public float Pitch => this.CurrentInputState.pitch;
    public float Yaw => this.CurrentInputState.yaw;
    public float Thrust => this.CurrentInputState.thrust;

    public bool IsShooting { get; private set; }

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
        var inputAxes = (x: Input.GetAxis("Horizontal"), y: Input.GetAxis("Vertical"));
        var mouseAxes = (x: Input.GetAxis("Mouse X"), y: Input.GetAxis("Mouse Y"));
        this.CurrentInputState = this.CalculateAppliedMovement(inputAxes, mouseAxes);
        this.IsShooting = Input.GetMouseButton(0);
    }

    private (float pitch, float roll, float yaw, float thrust, bool braking) CalculateAppliedMovement((float x, float y) inputAxes, (float x, float y) mouseAxes)
    {
        float yaw = 0, roll = 0, thrust = 0, pitch = 0;

        switch (this.xAxisMouseMode)
        {
            case HorizontalAxisMode.Roll:
                roll = mouseAxes.x;
                break;
            case HorizontalAxisMode.Yaw:
                yaw = mouseAxes.x;
                break;
        }

        switch (yAxisMouseMode)
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

        switch (xAxisMode)
        {
            case HorizontalAxisMode.Roll:
                roll = inputAxes.x;
                break;
            case HorizontalAxisMode.Yaw:
                yaw = inputAxes.x;
                break;
        }

        switch (yAxisMode)
        {
            case VerticalAxisMode.Pitch:
                pitch = inputAxes.y;
                break;
            case VerticalAxisMode.PitchInverted:
                pitch = -inputAxes.y;
                break;
            case VerticalAxisMode.Thrust:
                thrust = inputAxes.y;
                break;
        }

        var isBraking = Input.GetKey(this.brakingKey);

        return (pitch, roll, yaw, thrust, isBraking);

    }
}
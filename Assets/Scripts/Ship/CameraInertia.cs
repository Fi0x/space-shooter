using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInertia : MonoBehaviour
{

    [SerializeField, ReadOnlyInspector] private Vector3 originalLocalPosition;
    [SerializeField, ReadOnlyInspector] private Quaternion originalLocalRotation;


    [SerializeField] private Rigidbody shipRigidbody;
    [Header("Position Offset")]
    //
    [SerializeField] private AnimationCurve offsetMapping;
    /// <summary>
    /// Absolute "output". The InputMapping value gets run through the Offset mapping and multiplied by the maximumOffset.
    /// </summary>
    [SerializeField] private Vector3 maximumOffset;
    /// <summary>
    /// Absolute offset gets mapped onto [0;1]. Values over the maximum Value get clamped.
    /// </summary>
    [SerializeField] private Vector3 inputMapping;

    /// <summary>
    /// Lerp Value to use for Position Offsets
    /// </summary>
    [SerializeField, Range(0, 1)] private float lerpValue;



    [SerializeField] private ShipMovementHandler movementHandler;

    [Header("ReadOnly")]
    //
    [SerializeField, ReadOnlyInspector] private Vector3 currentPositionalOffset = Vector3.zero;
    [SerializeField, ReadOnlyInspector] private Vector3 currentInertiaVelocity = Vector3.zero;
    [SerializeField, ReadOnlyInspector] private Vector3 currentAngularInertia = Vector3.zero;


    [SerializeField] private Transform camera;

    private Vector3 velocityLastFrame = Vector3.zero;
    private Vector3 angularMomentumLastFrame = Vector3.zero;

    // Start is called before the first frame update
    private void Start()
    {
        this.originalLocalPosition = this.transform.localPosition;
        this.originalLocalRotation = this.transform.localRotation;
        if (this.camera == null)
        {
            this.camera = Camera.main?.gameObject.transform ??
                          throw new NullReferenceException("No camera set and no Camera tagged as Main");
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        this.HandlePosition();
        this.HandleRotation();
    }

    private void HandlePosition()
    {
        var velocityDifferenceToLastFrame = this.velocityLastFrame - this.shipRigidbody.velocity;
        this.velocityLastFrame = this.shipRigidbody.velocity;
        // The velocity "normalized" to one second
        var velocityDifferenceNormalized = velocityDifferenceToLastFrame / Time.fixedDeltaTime;
        // Value is divided by inputMapping
        var differenceScaledToInput = this.ScalePositionToInput(velocityDifferenceNormalized);
        var appliedMappingCurve = this.ApplyMappingCurveToInput(differenceScaledToInput);
        var appliedOutputMapping = Vector3.Scale(appliedMappingCurve, this.maximumOffset);
        var addedToOriginalLocalPosition = appliedOutputMapping + this.originalLocalPosition;
        this.transform.localPosition =
            Vector3.Lerp(this.transform.localPosition, addedToOriginalLocalPosition, this.lerpValue);
    }

    private Vector3 ApplyMappingCurveToInput((float x, float y, float z) input)
    {
        var x = this.offsetMapping.Evaluate(input.x);
        var y = this.offsetMapping.Evaluate(input.y);
        var z = this.offsetMapping.Evaluate(input.z);
        return new Vector3(x, y, z);
    }

    private (float x, float y, float z) ScalePositionToInput(Vector3 velocityDifferenceNormalized)
    {
        var x = velocityDifferenceNormalized.x / inputMapping.x;
        x = Mathf.Clamp(x, -1, 1);
        var y = velocityDifferenceNormalized.y / inputMapping.y;
        y = Mathf.Clamp(y, -1, 1);
        var z = velocityDifferenceNormalized.z / inputMapping.z;
        z = Mathf.Clamp(z, -1, 1);

        return (x, y, z);
    }

    private void HandleRotation()
    {
        currentAngularInertia = this.shipRigidbody.angularVelocity;
        var swapYZ = new Vector3(currentAngularInertia.x, currentAngularInertia.z, currentAngularInertia.y * 4f);

        this.gameObject.transform.localRotation = this.originalLocalRotation * Quaternion.Euler(-swapYZ);
    }



}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInertia : MonoBehaviour
{

    [SerializeField, ReadOnlyInspector] private Vector3 originalLocalPosition;
    [SerializeField, ReadOnlyInspector] private Quaternion originalLocalRotation;


    [SerializeField] private Rigidbody shipRigidbody;



    [SerializeField] private ShipMovementHandler movementHandler;

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
    }

    private void HandleRotation()
    {
        currentAngularInertia = this.shipRigidbody.angularVelocity;
        var swapYZ = new Vector3(currentAngularInertia.x, currentAngularInertia.z, currentAngularInertia.y * 4f);

        this.gameObject.transform.localRotation = this.originalLocalRotation * Quaternion.Euler(-swapYZ);
    }



}

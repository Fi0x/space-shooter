using System;
using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;

public class Speed : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private ShipMovementHandler smh;

    private SpeedIndicator speedIndicator;

    void Start()
    {
        this.speedIndicator = this.GetComponentInParent<SpeedIndicator>();

        this.speedIndicator.SetMaxSpeed(this.smh.maxSpeed);
    }

    private void Update()
    {
        this.UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        this.speedIndicator.SetCurrentSpeed(this.smh.currentSpeed);
    }
}

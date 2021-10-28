using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private ShipMovementHandler smh;

    private SpeedIndicator speedIndicator;

    void Start()
    {
        speedIndicator = GetComponentInParent<SpeedIndicator>();
        
        speedIndicator.SetMaxSpeed(smh.maxSpeed);
    }

    private void Update()
    {
        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        speedIndicator.SetCurrentSpeed(smh.desiredSpeed);
    }
}

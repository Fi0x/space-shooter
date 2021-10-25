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
        speedIndicator = GetComponentInChildren<SpeedIndicator>();
        
        speedIndicator.SetMaxSpeed(maxSpeed);
    }

    public void ChangeSpeed()
    {
        speedIndicator.SetCurrentSpeed(currentSpeed);
    }
}

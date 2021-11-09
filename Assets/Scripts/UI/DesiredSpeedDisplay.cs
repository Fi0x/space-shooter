using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesiredSpeedDisplay : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private ShipMovementHandler smh;
    [SerializeField] private Image indicator;
    [SerializeField] private float maxValue = 175;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = indicator.transform.localPosition;
    }

    void Update()
    {
        var thrustPercent = smh.desiredSpeed / smh.maxSpeed;
        indicator.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y + thrustPercent * maxValue, originalPosition.z);
    }
}

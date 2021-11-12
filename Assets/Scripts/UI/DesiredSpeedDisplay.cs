using System;
using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using UnityEngine.UI;

public class DesiredSpeedDisplay : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private ShipMovementHandler smh;
    [SerializeField] private Image indicator;
    [SerializeField] private float maxValue = 175;

    private Vector3 originalPosition;

    private void Start()
    {
        this.originalPosition = this.indicator.transform.localPosition;
    }

    private void Update()
    {
        var thrustPercent = this.smh.desiredSpeed / (this.smh.maxSpeed + this.smh.maxSpeedBoost);
        this.indicator.transform.localPosition = new Vector3(this.originalPosition.x, this.originalPosition.y + thrustPercent * this.maxValue, this.originalPosition.z);
    }
}

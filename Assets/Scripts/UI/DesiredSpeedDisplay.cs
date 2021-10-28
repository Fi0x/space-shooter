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

    // Update is called once per frame
    void Update()
    {
        float thrustPercent = smh.currentSpeed / smh.maxSpeed;
        indicator.transform.up = new Vector3(thrustPercent * maxValue, 0, 0);
    }
}

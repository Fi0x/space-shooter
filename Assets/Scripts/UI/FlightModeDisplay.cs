using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using UnityEngine.UI;

public class FlightModeDisplay : MonoBehaviour
{
    [SerializeField] private ShipMovementHandler smh;
    [SerializeField] private Text textComponent;

    void Update()
    {
        textComponent.text = $"Flight control mode: {smh.currentFlightModel}";
    }
}

using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using UnityEngine.UI;

public class FlightModeDisplay : MonoBehaviour
{
    [SerializeField] private ShipMovementHandler2 smh;
    [SerializeField] private Text textComponent;

    void Update()
    {
        this.textComponent.text = $"Flight control mode: {this.smh.Settings.ProfileName}";
    }
}

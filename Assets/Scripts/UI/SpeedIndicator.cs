using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedIndicator : MonoBehaviour
{

    [Header("SpeedIndicator")]
    [SerializeField] private Slider slider;

    private float offset = 30;

    void Start()
    {
        slider.value = offset;
    }

    public void SetMaxSpeed(float maxSpeed)
    {
        offset = maxSpeed;
        slider.maxValue = 2 * maxSpeed;
    }

    public void SetCurrentSpeed(float speed)
    {
        var calculatedValue = speed + offset;
        if (calculatedValue < 0) calculatedValue = 0;
        if (calculatedValue > slider.maxValue) calculatedValue = slider.maxValue;
        slider.value = calculatedValue;
    }
}

using System;
using Ship;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DesiredSpeedDisplay : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private ShipMovementHandler smh;
        [SerializeField] private Image indicator;
        [SerializeField] private float maxValue = 175;

        private Vector3 originalPosition;
        private float fractionToDisplay = 0f;

        private void OnEnable()
        {
            this.smh.DesiredSpeedChangedEvent += this.HandleDesiredSpeedChangedEvent;
        }

        private void HandleDesiredSpeedChangedEvent(float speed, float maxSpeed)
        {
            this.fractionToDisplay = speed / maxSpeed;
            Debug.Log(this.fractionToDisplay);
        }

        private void OnDisable()
        {
            this.smh.DesiredSpeedChangedEvent -= this.HandleDesiredSpeedChangedEvent;
        }




        private void Start()
        {
            this.originalPosition = this.indicator.transform.localPosition;
        }

        private void Update()
        {
            var yOffset = this.maxValue * this.fractionToDisplay;
            //var yOffset = this.maxValue * this.fractionToDisplay;
            this.indicator.transform.localPosition = new Vector3(this.originalPosition.x, this.originalPosition.y + yOffset, this.originalPosition.z);
        }
    }
}
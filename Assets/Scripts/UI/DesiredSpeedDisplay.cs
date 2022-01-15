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

        private InputHandler inputHandler;
        private Vector3 originalPosition;
        private float fractionToDisplay = 0.5f;

        private void OnEnable()
        {
            this.smh.DesiredSpeedChangedEvent += this.HandleDesiredSpeedChangedEvent;
        }

        private void HandleDesiredSpeedChangedEvent(float speed, float maxSpeed)
        {
            this.fractionToDisplay = speed / maxSpeed;
        }

        private void OnDisable()
        {
            this.smh.DesiredSpeedChangedEvent -= this.HandleDesiredSpeedChangedEvent;
        }




        private void Start()
        {
            this.originalPosition = this.indicator.transform.localPosition;
            this.inputHandler = this.smh.InputHandler;
        }

        private void Update()
        {
            var yOffset = this.maxValue * (this.inputHandler.IsBoosting ? 1 : this.fractionToDisplay);
            this.indicator.transform.localPosition = new Vector3(this.originalPosition.x, this.originalPosition.y + yOffset, this.originalPosition.z);
        }
    }
}
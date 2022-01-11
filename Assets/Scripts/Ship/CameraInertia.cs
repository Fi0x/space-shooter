using System;
using UnityEngine;

namespace Ship
{
    public class CameraInertia : MonoBehaviour
    {


        [SerializeField, ReadOnlyInspector] private Vector3 originalLocalPosition;
        [SerializeField, ReadOnlyInspector] private Quaternion originalLocalRotation;


        [SerializeField] private Rigidbody shipRigidbody;

        /// <summary>
        /// Function maps from [0,1] on x to [0,1] on f(x)
        /// </summary>
        [Header("Position Offset")]
        //
        [SerializeField] private AnimationCurve offsetMapping;

        /// <summary>
        /// Absolute "output". The InputMapping value gets run through the Offset mapping and multiplied by the maximumOffset.
        /// </summary>
        [SerializeField] private Vector3 maximumOffset;
        /// <summary>
        /// Absolute offset gets mapped onto [0;1]. Values over the maximum Value get clamped.
        /// </summary>
        [SerializeField] private Vector3 inputMapping;

        /// <summary>
        /// Lerp Value to use for Position Offsets
        /// </summary>
        [SerializeField, Range(0, 1)] private float lerpValue;



        [SerializeField] private ShipMovementHandler2 movementHandler;

        [Header("ReadOnly")]
        //
        [SerializeField, ReadOnlyInspector] private Vector3 currentAngularInertia = Vector3.zero;
        [SerializeField, ReadOnlyInspector] private Vector3 currentPositionalInertial = Vector3.zero;

        [SerializeField] private Transform camera;

        private Vector3 velocityLastFrame = Vector3.zero;

        public event Action<Vector3> CameraUpdatedPositionEvent;

        // Start is called before the first frame update
        private void Start()
        {
            var tf = this.transform;
            this.originalLocalPosition = tf.localPosition;
            this.originalLocalRotation = tf.localRotation;
            if (this.camera == null)
            {
                this.camera = Camera.main?.gameObject.transform ??
                              throw new NullReferenceException("No camera set and no Camera tagged as Main");
            }

            this.movementHandler.ForcesAppliedEvent += this.HandleForcesAppliedEvent;
        }

        // Update is called once per frame
        private void HandleForcesAppliedEvent()
        {
            this.HandlePosition();
            this.HandleRotation();
            this.CameraUpdatedPositionEvent?.Invoke(this.transform.position);
        }

        private void HandlePosition()
        {
            var shipVelocity = this.shipRigidbody.velocity;
            var velocityDifferenceToLastFrame = this.velocityLastFrame - shipVelocity;
            this.velocityLastFrame = shipVelocity;
            var velocityDifferenceToLastFrameInLocalPosition =
                this.transform.InverseTransformDirection(velocityDifferenceToLastFrame);
            // The velocity "normalized" to one second
            var velocityDifferenceNormalized = velocityDifferenceToLastFrameInLocalPosition / Time.fixedDeltaTime;
            // Value is divided by inputMapping
            var differenceScaledToInput = this.ScalePositionToInput(velocityDifferenceNormalized);
            var appliedMappingCurve = this.ApplyMappingCurveToInput(differenceScaledToInput);
            this.currentPositionalInertial = appliedMappingCurve;
            var appliedOutputMapping = Vector3.Scale(appliedMappingCurve, this.maximumOffset);
            var positionToLerpFrom = appliedOutputMapping + this.originalLocalPosition;



            this.transform.localPosition =
                Vector3.Lerp(this.transform.localPosition,positionToLerpFrom, this.lerpValue);

        }

        private Vector3 ApplyMappingCurveToInput((float x, float y, float z) input)
        {
            var xAbs = this.offsetMapping.Evaluate(Mathf.Abs(input.x));
            var yAbs = this.offsetMapping.Evaluate(Mathf.Abs(input.y));
            var zAbs = this.offsetMapping.Evaluate(Mathf.Abs(input.z));
            return new Vector3(
                input.x > 0 ? xAbs : -xAbs,
                input.y > 0 ? yAbs : -yAbs,
                input.z > 0 ? zAbs : -zAbs
            );

        }

        private (float x, float y, float z) ScalePositionToInput(Vector3 velocityDifferenceNormalized)
        {
            var x = velocityDifferenceNormalized.x / this.inputMapping.x;
            x = Mathf.Clamp(x, -1, 1);
            var y = velocityDifferenceNormalized.y / this.inputMapping.y;
            y = Mathf.Clamp(y, -1, 1);
            var z = velocityDifferenceNormalized.z / this.inputMapping.z;
            z = Mathf.Clamp(z, -1, 1);

            return (x, y, z);
        }

        private void HandleRotation()
        {
            this.currentAngularInertia = this.shipRigidbody.angularVelocity;
            var swapYZ = new Vector3(this.currentAngularInertia.x, this.currentAngularInertia.z, this.currentAngularInertia.y * 4f);

            this.gameObject.transform.localRotation = this.originalLocalRotation * Quaternion.Euler(-swapYZ);
        }

        private void OnDestroy()
        {
            if (this.movementHandler)
            {
                this.movementHandler.ForcesAppliedEvent -= this.HandleForcesAppliedEvent;
            }
        }
    }
}

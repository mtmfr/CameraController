using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    [Serializable]
    public class Camera2DLookAheadControl
    {
        [Tooltip("The input of the look ahead.")]
        [SerializeField] private InputActionReference input;
        [Tooltip("Wether the camera has look ahead.")]
        [SerializeField] private bool canLookAhead;
        [Tooltip("The maximum value of the look ahead.")]
        [SerializeField] private Vector2 lookAhead;
        [SerializeField] SmoothingParameters lookAheadSmoothing;

        /// <summary>
        /// The input of the look ahead.
        /// </summary>
        public InputAction action => input ? input : null;
        /// <summary>
        /// Wether the look ahead can ba applied.
        /// </summary>
        public bool hasLookAhead => canLookAhead;
        /// <summary>
        /// the maximum value of the look ahead. Each value represent both the maximum and minimum.
        /// </summary>
        public Vector2 maxLookAhead => lookAhead;
        /// <summary>
        /// The duration of the smoothing.
        /// </summary>
        public float smoothingTime => lookAheadSmoothing.smoothingTime;
        /// <summary>
        /// The easing applied to the smoothing.
        /// </summary>
        public EaseType easingType => lookAheadSmoothing.EasingType;

        public Camera2DLookAheadControl(bool canLookAhead, Vector2 lookAhead, float smoothingTime, EaseType easingType, InputActionReference input = null)
        {
            this.canLookAhead = hasLookAhead;
            this.input = input;
            this.lookAhead = lookAhead;
            lookAheadSmoothing = new(smoothingTime, easingType);
        }

        public void EnableLookAhead()
        {
            action?.Enable();
            canLookAhead = true;
        }

        public void DisableLookAhead()
        {
            action?.Disable();
            canLookAhead = false;
        }
    }
}

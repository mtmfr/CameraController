using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    [Serializable]
    public class Camera2DLookAheadControl
    {
        [SerializeField] private InputActionReference input;
        [SerializeField] private bool canLookAhead;
        [SerializeField] private Vector2 lookAhead;

        [SerializeField] float smoothTime;
        [SerializeField] private EaseType easeType;

        public InputAction action => input ? input : null;
        public bool hasLookAhead => canLookAhead;
        public Vector2 maxLookAhead => lookAhead;
        public float smoothingTime => smoothTime;
        public EaseType easingType => easeType;

        private Camera2DLookAheadControl(bool canLookAhead, Vector2 lookAhead, float smoothingTime, EaseType easingType, InputActionReference input = null)
        {
            this.canLookAhead = hasLookAhead;
            this.input = input;
            this.lookAhead = lookAhead;
            smoothTime = smoothingTime;
            easeType = easingType;
        }

        public static Camera2DLookAheadControl Create(bool hasLookAhead, Vector2 maxLookAhead, float smoothingTime, EaseType easingType, InputActionReference input = null)
        {
            return new(hasLookAhead, maxLookAhead, smoothingTime, easingType, input);
        }
    }
}

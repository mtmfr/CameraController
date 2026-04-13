using CameraController;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    [Serializable]
    public class CameraInput
    {
        [SerializeField] private bool canControlCamera;
        [SerializeField] private InputActionReference controlInput;
        [SerializeField] private Vector2 sensitivity;

        public bool canCameraBeControlled => canControlCamera;
        public InputAction lookAction => controlInput ? controlInput.action : null;
        public float horizontalSensitivity
        {
            get
            {
                return sensitivity.x;
            }
            set
            {
                sensitivity.x = value < 0.1f ? 0.1f : value;
            }
        }

        public float verticalSensitivity
        {
            get => sensitivity.y;
            set => sensitivity.y = value < 0.1f ? 0.1f : value;
        }

        private CameraInput(bool canControlCamera, InputActionReference controlInput, Vector2 sensitivity)
        {
            this.canControlCamera = canControlCamera;
            this.controlInput = controlInput;
            this.sensitivity = sensitivity;
        }

        public static CameraInput CreateInputParameter(bool canBeControlled, float horizontalSensitivity, float verticalSensitivity, InputActionReference actionInput = null)
        {
            if (horizontalSensitivity < 0.1f)
                horizontalSensitivity = 0.1f;
            if (verticalSensitivity < 0.1f)
                verticalSensitivity = 0.1f;

            return new(canBeControlled, actionInput, new Vector2(horizontalSensitivity, verticalSensitivity));
        }

        public void EnableCameraControl()
        {
            if (lookAction == null)
                return;

            lookAction.Enable();
        }
        public void DisableCameraControl()
        {
            if (lookAction == null)
                return;

            lookAction.Disable();
        }
    }
}
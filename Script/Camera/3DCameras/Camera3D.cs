using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    public abstract class Camera3D : BaseCamera
    {
        private Quaternion yawRotation = Quaternion.identity;
        private Quaternion pitchRotation = Quaternion.identity;

        /// <summary>
        /// The rotation of the controller on the y axis
        /// </summary>
        public Quaternion yaw => yawRotation;
        /// <summary>
        /// The rotation of the controller on the x axis
        /// </summary>
        public Quaternion pitch => pitchRotation;

        /// <summary>
        /// The rotation of the controller
        /// </summary>
        public Quaternion rotation => yaw * pitch;

        [Header("Rotation")]
        [Tooltip("Limits of the rotation on the x axis of the camera")]
        [SerializeField] private RotationLimits pitchRotationLimits;
        [Tooltip("Limits of the rotation on the y axis of the camera")]
        [SerializeField] private RotationLimits yawRotationLimits;

        [Header("Input")]
        [SerializeField] private CameraInput inputControl;
        private Vector2 lookInputDir;

        protected virtual void OnEnable()
        {
#if (UNITY_EDITOR)
            if (!EditorApplication.isPlaying)
                return;
#endif
            if (inputControl.lookAction == null)
                return;

            inputControl.lookAction.Enable();
            inputControl.lookAction.performed += GetMovementDir;
        }

        private void OnDisable()
        {
#if (UNITY_EDITOR)
            if (!EditorApplication.isPlaying)
                return;
#endif

            if (inputControl.lookAction == null)
                return;

            inputControl.lookAction.Disable();
            inputControl.lookAction.performed -= GetMovementDir;
        }

        private void GetMovementDir(InputAction.CallbackContext context)
        {
#if (UNITY_EDITOR)
            if (!EditorApplication.isPlaying)
                return;
#endif
            try
            {
                lookInputDir = context.ReadValue<Vector2>();
            }
            catch (InvalidOperationException)
            {
                Debug.LogError($"The return type of {inputControl.lookAction.name} is not Vector2 but {inputControl.lookAction.activeValueType}");
            }
        }

        protected override void UpdateCamera()
        {
            if (target == null)
                return;

            SetTargetFollowerRotation();
            base.UpdateCamera();
        }

        /// <summary>
        /// Update the rotation of the the target follower
        /// </summary>
        private void SetTargetFollowerRotation()
        {
            Quaternion yawRotationToApply = Quaternion.Euler(0, lookInputDir.x * inputControl.horizontalSensitivity, 0);
            Quaternion pitchRotationToApply = Quaternion.Euler(-lookInputDir.y * inputControl.verticalSensitivity, 0, 0);

            yawRotation = GetNewRotation(yawRotation, yawRotationToApply, Vector3.up, yawRotationLimits);
            pitchRotation = GetNewRotation(pitchRotation, pitchRotationToApply, Vector3.right, pitchRotationLimits);

            Quaternion newRotation = yawRotation * pitchRotation;
            targetFollower.rotation = newRotation;
        }

        #region target follower rotation control
        /// <summary>
        /// Get the next rotation on the specified axis
        /// </summary>
        /// <param name="currentRotation">the current rotation</param>
        /// <param name="rotationToAdd">the ammount of rotation to add</param>
        /// <param name="axis">the axis of the rotation</param>
        /// <param name="rotationLimits">the limits of the rotation</param>
        /// <returns>The product of the current rotation * the rotation to add</returns>
        private Quaternion GetNewRotation(Quaternion currentRotation, Quaternion rotationToAdd, Vector3 axis, RotationLimits rotationLimits)
        {
            Quaternion minRotation = Quaternion.Euler(axis * rotationLimits.MinAngle);
            Quaternion maxRotation = Quaternion.Euler(axis * rotationLimits.MaxAngle);

            Quaternion rotationToCheck = currentRotation * rotationToAdd;

            Quaternion newRotation;

            if (!IsWithinAllowedRotation(rotationToCheck, minRotation, maxRotation))
                newRotation = GetLimitToBeAt(currentRotation, minRotation, maxRotation, rotationLimits.Wrap);
            else
                newRotation = rotationToCheck;

            return newRotation;
        }

        /// <summary>
        /// Check wether the next rotation of the camera is within the limits
        /// </summary>
        /// <param name="nextRotation">The new rotation of the camera</param>
        /// <param name="minAllowedRotation">The smallest allowed rotation</param>
        /// <param name="maxAllowedRotation">The greatest allowed rotation</param>
        /// <returns>True if the next rotation is within the allowed angle, false otherwise</returns>
        private bool IsWithinAllowedRotation(Quaternion nextRotation, Quaternion minAllowedRotation, Quaternion maxAllowedRotation)
        {
            //The dot product of the minimum allowed rotation the quaternion identity
            float minIdentityDot = Quaternion.Dot(minAllowedRotation, Quaternion.identity);
            //Dot product of the minimum allowed rotation and the maximum allowed rotation
            float minMaxDot = Quaternion.Dot(minAllowedRotation, maxAllowedRotation);

            //the ">0" allow to ignore a rotation of -180 or 180 degree
            if (Mathf.Abs(minIdentityDot) > 0)
            {
                //Check the side of the rotation to prevent the false positive with the inverse rotation
                if (Quaternion.Dot(nextRotation, maxAllowedRotation) < minMaxDot)
                    return false;
            }

            if (Mathf.Abs(Quaternion.Dot(maxAllowedRotation, Quaternion.identity)) > 0)
            {
                if (Quaternion.Dot(nextRotation, minAllowedRotation) < minMaxDot)
                    return false;
            }

            //Case for a rotation equal to -180 or 180 degree
            if (Quaternion.Dot(nextRotation, Quaternion.identity) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Get the limit the given rotation should go at
        /// </summary>
        /// <param name="currentRotation">th current rotation</param>
        /// <param name="minRotation">the minimum allowed rotation</param>
        /// <param name="maxRotation">the maximum allowed rotation</param>
        /// <param name="wrap">Wether the rotation should stay at the reached limit or go at the opposite limit.
        /// </param>
        private Quaternion GetLimitToBeAt(Quaternion currentRotation, Quaternion minRotation, Quaternion maxRotation, bool wrap)
        {
            if (currentRotation == minRotation)
                return wrap ? maxRotation : minRotation;

            if (currentRotation == maxRotation)
                return wrap ? minRotation : maxRotation;

            float dotProduct = Quaternion.Dot(currentRotation, maxRotation);

            if (dotProduct < 0)
                return wrap ? maxRotation : minRotation;

            float minDotProduct = Quaternion.Dot(minRotation, maxRotation);

            float x = Mathf.InverseLerp(minDotProduct, 1, dotProduct);

            if (x < 0.5f)
                return wrap ? maxRotation : minRotation;
            else
                return wrap ? minRotation : maxRotation;
        }
        #endregion

        /// <summary>
        /// Set the limits of the yaw and pitch rotation of the target follower
        /// </summary>
        /// <param name="newYawLimits"></param>
        /// <param name="newPitchLimits"></param>
        protected void SetRotationLimits(RotationLimits newYawLimits, RotationLimits newPitchLimits)
        {
            yawRotationLimits = newYawLimits;
            pitchRotationLimits = newPitchLimits;
        }

        protected void SetInputControl(CameraInput newInputControl) => inputControl = newInputControl;
    }
}
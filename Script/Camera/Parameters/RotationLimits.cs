using System;
using UnityEngine;

namespace CameraController
{
    [Serializable]
    public struct RotationLimits : ISerializationCallbackReceiver
    {
        [Tooltip("The smallest allowed angle of the camera")]
        [SerializeField, Clamp(-180, 180)] private float minAngleValue;

        [Tooltip("The largest allowed angle of the camera")]
        [SerializeField, Clamp(-180, 180)] private float maxAngleValue;

        [Tooltip("Should the rotation continue when reaching a limit")]
        [SerializeField] private bool wrap;

        private float preserializedMinAngle;
        private float preserializedMaxAngle;

        /// <summary>
        /// The smallest allowed angle of the rotation
        /// </summary>
        public readonly float MinAngle => minAngleValue;
        /// <summary>
        /// The largest allowed angle of the rotation
        /// </summary>
        public readonly float MaxAngle => maxAngleValue;
        /// <summary>
        /// Should the rotation continue when reaching a limit
        /// </summary>
        public readonly bool Wrap => wrap;

        public RotationLimits(float minAngle, float maxAngle, bool wrap)
        {
            minAngleValue = Mathf.Clamp(minAngle, -180f, 180f);
            maxAngleValue = Mathf.Clamp(maxAngle, -180f, 180f);
            preserializedMinAngle = Mathf.Clamp(minAngle, -180f, 180f);
            preserializedMaxAngle = maxAngle;

            this.wrap = wrap;
        }

        public void OnAfterDeserialize()
        {
            bool minChanged = !Mathf.Approximately(minAngleValue, preserializedMinAngle);
            bool maxChanged = !Mathf.Approximately(maxAngleValue, preserializedMaxAngle);

            if (minAngleValue > maxAngleValue)
            {
                if (minChanged)
                {
                    minAngleValue = preserializedMaxAngle;
                }
                else if (maxChanged)
                {
                    maxAngleValue = preserializedMinAngle;
                }
            }

            preserializedMinAngle = minAngleValue;
            preserializedMaxAngle = maxAngleValue;
        }

        public void OnBeforeSerialize()
        {
            minAngleValue = preserializedMinAngle;
            maxAngleValue = preserializedMaxAngle;
        }
    }
}

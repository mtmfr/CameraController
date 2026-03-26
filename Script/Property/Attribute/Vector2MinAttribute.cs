using UnityEngine;

namespace CameraController
{
    public class Vector2MinAttribute : PropertyAttribute
    {
        public readonly float yMin;
        public readonly float xMin;

        public Vector2MinAttribute(float yMin, float xMin)
        {
            this.yMin = yMin;
            this.xMin = xMin;
        }
    }
}


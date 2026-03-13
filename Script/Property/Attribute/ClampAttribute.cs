using UnityEngine;

namespace CameraController
{
    public class ClampAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;

        public ClampAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public ClampAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
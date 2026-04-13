using UnityEngine;

namespace CameraController
{
    public static class Smoothing
    {
        /// <summary>
        /// Give a value between the start and end value 
        /// </summary>
        /// <param name="startValue">the original value</param>
        /// <param name="endValue">The ending value </param>
        /// <param name="currentValue">The current value</param>
        /// <param name="smoothingTime">The time it takes to go from startValue to endValue</param>
        /// <param name="easeType">The easing to apply</param>
        /// <param name="deltaTime">By wich ammount the change is made. -1 is Time.deltaTime</param>
        /// <returns>A new value going towards the end value</returns>
        public static float SmoothValue(float startValue, float endValue, float currentValue, float smoothingTime, EaseType easeType, float deltaTime = -1)
        {
            if (smoothingTime == 0)
                return endValue;

            if (deltaTime < 0)
                deltaTime = Time.deltaTime;

            return easeType switch
            {
                EaseType.EaseIn => EaseInSmoothing(startValue, endValue, currentValue, smoothingTime, deltaTime),
                EaseType.EaseOut => EaseOutSmoothing(startValue, endValue, currentValue, smoothingTime, deltaTime),
                EaseType.EaseInThenEaseOut => EaseInSmoothing(startValue, endValue, currentValue, smoothingTime, deltaTime),
                _ => LinearSmoothing(startValue, endValue, currentValue, smoothingTime, deltaTime)
            };
        }

        private static float LinearSmoothing(float startValue, float endValue, float currentValue, float smoothingTime, float deltaTime)
        {
            if (Mathf.Approximately(endValue, currentValue))
                return endValue;

            float t = Mathf.InverseLerp(startValue, endValue, currentValue);

            t += deltaTime / smoothingTime;
            if (t > 1f)
                t = 1f;

            float newValue = startValue + (endValue - startValue) * t;

            return newValue;
        }

        private static float EaseInSmoothing(float startValue, float endValue, float currentValue, float smoothingTime, float deltaTime)
        {
            if (Mathf.Approximately(endValue, currentValue))
                return endValue;

            float currentT = Mathf.InverseLerp(startValue, endValue, currentValue);

            float currentTime = smoothingTime * (2 * Mathf.Acos(1 - currentT) / Mathf.PI);

            currentTime = Mathf.Clamp(currentTime + deltaTime, -smoothingTime, smoothingTime);

            float newT = 1 - Mathf.Cos(currentTime / smoothingTime * Mathf.PI / 2);

            float newValue = startValue + (endValue - startValue) * newT;

            return newValue;
        }

        private static float EaseOutSmoothing(float startValue, float endValue, float currentValue, float smoothingTime, float deltaTime)
        {
            if (Mathf.Approximately(endValue, currentValue))
                return endValue;

            float currentT = Mathf.InverseLerp(startValue, endValue, currentValue);

            float currentTime = smoothingTime * (2 * Mathf.Asin(currentT) / Mathf.PI);

            currentTime = Mathf.Clamp(currentTime + deltaTime, -smoothingTime, smoothingTime);

            float newT = Mathf.Sin(currentTime / smoothingTime * Mathf.PI / 2);

            float newValue = startValue + (endValue - startValue) * newT;

            return newValue;
        }

        private static float EaseInThenEaseOutSmoothing(float startValue, float endValue, float currentValue, float smoothingTime, float deltaTime)
        {
            if (Mathf.Approximately(endValue, currentValue))
                return endValue;

            float currentT = Mathf.InverseLerp(startValue, endValue, currentValue);

            float currentTime = (Mathf.Acos(-2 * currentT + 1) / Mathf.PI) * smoothingTime;

            currentTime = Mathf.Clamp(currentTime + deltaTime, 0, smoothingTime);

            float newT = -(Mathf.Cos(currentTime / smoothingTime * Mathf.PI) - 1) / 2;

            float newValue = startValue + (endValue - startValue) * newT;

            return newValue;
        }

        /// <summary>
        /// Give a new vector between startPosition and endPosition
        /// </summary>
        /// <param name="startPosition">the position the object we want to move originaly have</param>
        /// <param name="endPosition">The position we want to reach</param>
        /// <param name="currentPosition">The position the moving object is at</param>
        /// <param name="smoothingTime">The time it takes to go from startPosition to endPosition</param>
        /// <param name="easeType">The easing that will be applied</param>
        /// <param name="deltaTime">The rate at wich the changes are made. At -1 Time.deltatime is used</param>
        /// <returns>The new value of the given vector</returns>
        public static Vector3 SmoothPosition(Vector3 startPosition, Vector3 endPosition, Vector3 currentPosition, float smoothingTime, EaseType easeType, float deltaTime = -1)
        {
            if (smoothingTime == 0)
                return endPosition;

            if (deltaTime < 0)
                deltaTime = Time.deltaTime;

            return easeType switch
            {
                EaseType.EaseIn => EaseInSmoothing(startPosition, endPosition, currentPosition, smoothingTime, deltaTime),
                EaseType.EaseOut => EaseOutSmoothing(startPosition, endPosition, currentPosition, smoothingTime, deltaTime),
                EaseType.EaseInThenEaseOut => EaseInThenEaseOutSmoothing(startPosition, endPosition, currentPosition, smoothingTime, deltaTime),
                _ => LinearSmoothing(startPosition, endPosition, currentPosition, smoothingTime, deltaTime),
            };
        }

        private static Vector3 LinearSmoothing(Vector3 startPosition, Vector3 endPosition, Vector3 currentPosition, float smoothingTime, float deltaTime)
        {
            float x = LinearSmoothing(startPosition.x, endPosition.x, currentPosition.x, smoothingTime, deltaTime);
            float y = LinearSmoothing(startPosition.y, endPosition.y, currentPosition.y, smoothingTime, deltaTime);
            float z = LinearSmoothing(startPosition.z, endPosition.z, currentPosition.z, smoothingTime, deltaTime);

            return new(x, y, z);
        }

        private static Vector3 EaseInSmoothing(Vector3 startPosition, Vector3 endPosition, Vector3 currentPosition, float smoothingTime, float deltaTime)
        {
            float x = EaseInSmoothing(startPosition.x, endPosition.x, currentPosition.x, smoothingTime, deltaTime);
            float y = EaseInSmoothing(startPosition.y, endPosition.y, currentPosition.y, smoothingTime, deltaTime);
            float z = EaseInSmoothing(startPosition.z, endPosition.z, currentPosition.z,smoothingTime, deltaTime);

            return new(x, y, z);
        }

        private static Vector3 EaseOutSmoothing(Vector3 startPosition, Vector3 endPosition, Vector3 currentPosition, float smoothingTime, float deltaTime)
        {
            float x = EaseOutSmoothing(startPosition.x, endPosition.x, currentPosition.x, smoothingTime, deltaTime);
            float y = EaseOutSmoothing(startPosition.y, endPosition.y, currentPosition.y, smoothingTime, deltaTime);
            float z = EaseOutSmoothing(startPosition.z, endPosition.z, currentPosition.z, smoothingTime, deltaTime);

            return new(x, y, z);
        }

        private static Vector3 EaseInThenEaseOutSmoothing(Vector3 startPosition, Vector3 endPosition, Vector3 currentPosition, float smoothingTime, float deltaTime)
        {
            float x = EaseInThenEaseOutSmoothing(startPosition.x, endPosition.x, currentPosition.x, smoothingTime, deltaTime);
            float y = EaseInThenEaseOutSmoothing(startPosition.y, endPosition.y, currentPosition.y, smoothingTime, deltaTime);
            float z = EaseInThenEaseOutSmoothing(startPosition.z, endPosition.z, currentPosition.z, smoothingTime, deltaTime);

            return new(x, y, z);
        }
    }
}
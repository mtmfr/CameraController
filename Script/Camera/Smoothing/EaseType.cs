namespace CameraController
{
    public enum EaseType : byte
    {
        /// <summary>
        /// Constant speed
        /// </summary>
        Linear,
        /// <summary>
        /// Accelerate the closer it is from the target
        /// </summary>
        EaseIn,
        /// <summary>
        /// Decelerate the closer it is to the target
        /// </summary>
        EaseOut,
        /// <summary>
        /// First accelerate the decelerate when getting closer to the target
        /// </summary>
        EaseInThenEaseOut
    }
}
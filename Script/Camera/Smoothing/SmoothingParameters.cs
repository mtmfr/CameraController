using CameraController;
using System;
using UnityEngine;

[Serializable]
public struct SmoothingParameters
{
    [Tooltip("How long the smoothing last. 0 = no smoothing")]
    [SerializeField, Min(0)] private float smoothTime;
    [Tooltip("The easing applied to the smoothing.")]
    [SerializeField] private EaseType easeType;

    /// <summary>
    /// Duration of the smoothing.
    /// </summary>
    public readonly float smoothingTime => smoothTime;
    /// <summary>
    /// The easing applied to the smoothing.
    /// </summary>
    public readonly EaseType EasingType => easeType;

    public SmoothingParameters(float smoothingTime, EaseType easeType)
    {
        smoothTime = smoothingTime;
        this.easeType = easeType;
    }
}

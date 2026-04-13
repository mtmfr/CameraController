using System;
using UnityEngine;

namespace CameraController
{
    [Serializable]
    public struct CollisionDetection
    {
        [SerializeField] private float collisionRadius;
        [Tooltip("Define which layer the camera should collide with")]
        [SerializeField] private LayerMask collisionMask;
        [Tooltip("Define if the colliders mark as trigger should be counted as a collision")]
        [SerializeField] private QueryTriggerInteraction queryParameters;

        /// <summary>
        /// The radius of the collision
        /// </summary>
        public readonly float colliderRadius => collisionRadius;

        /// <summary>
        /// Which layer does the collision check
        /// </summary>
        public readonly LayerMask mask => collisionMask;

        /// <summary>
        /// How to treat trigger colliders
        /// </summary>
        public readonly QueryTriggerInteraction collisionQuery => queryParameters;

        public CollisionDetection(float collisionRadius, LayerMask collisionMask, QueryTriggerInteraction queryParameters)
        {
            this.collisionRadius = collisionRadius;
            this.collisionMask = collisionMask;
            this.queryParameters = queryParameters;
        }
    }

    [Serializable]
    public struct SmoothingParameters
    {
        [Tooltip("How strong the applied damping is. 0 = no damping")]
        [SerializeField, Min(0)] private float smoothTime;
        [Tooltip("How the damping is applied the closer the object is from it's target")]
        [SerializeField] private EaseType easeType;

        public readonly float smoothingTime => smoothTime;
        public readonly EaseType EasingType => easeType;

        public SmoothingParameters(float smoothingTime, EaseType easeType)
        {
            smoothTime = smoothingTime;
            this.easeType = easeType;
        }
    }

    /// <summary>
    ///When should the camera movement operate
    /// </summary>
    public enum FollowMode : byte
    {
        Update,
        FixedUpdate,
        LateUpdate,
    }
}

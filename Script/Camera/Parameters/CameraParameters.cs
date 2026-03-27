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
    public struct DampingParams
    {
        [Tooltip("How strong the applied damping is. 0 = no damping")]
        [SerializeField, Min(0)] private float damping;
        [Tooltip("How the damping is applied the closer the object is from it's target")]
        [SerializeField] private EaseType easeType;

        public readonly float Damping => damping;
        public readonly EaseType EasingType => easeType;

        public DampingParams(float damping, EaseType easeType)
        {
            damping = Mathf.Clamp01(damping);
            this.damping = damping;
            this.easeType = easeType;
        }

        [Serializable]
        public enum EaseType : byte
        {
            /// <summary>
            /// Get closer the closest it is from the target
            /// </summary>
            EaseIn,
            /// <summary>
            /// Get slower the closer it is from the target
            /// </summary>
            EaseOut,
            /// <summary>
            /// Get faster when approaching the target then gets slower the closer it gets to it
            /// </summary>
            EaseInThenEaseOut
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

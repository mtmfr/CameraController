using System;
using UnityEngine;

namespace CameraController
{
    [Serializable]
    public struct CollisionDetection
    {
        [Tooltip("The radius of the collider of the camera")]
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

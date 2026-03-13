using System;
using UnityEngine;

namespace CameraController
{
    [Serializable]
    public struct CollisionDetection
    {
        [SerializeField] private float collisionRadius;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private QueryTriggerInteraction queryParameters;
        [SerializeField] private bool ignoreTarget;

        public readonly float colliderRadius => collisionRadius;
        public readonly LayerMask mask => collisionMask;
        public readonly QueryTriggerInteraction collisionQuery => queryParameters;
        public readonly bool shouldIgnoreTarget => ignoreTarget;

        public CollisionDetection(float collisionRadius, LayerMask collisionMask, QueryTriggerInteraction queryParameters, bool shouldIgnoreTarget)
        {
            this.collisionRadius = collisionRadius;
            this.collisionMask = collisionMask;
            this.queryParameters = queryParameters;
            this.ignoreTarget = shouldIgnoreTarget;
        }
    }

    [Serializable]
    public struct DampingParams
    {
        [SerializeField, Min(0)] private float damping;
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
            EaseIn,
            EaseOut,
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

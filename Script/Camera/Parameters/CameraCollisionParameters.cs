using System;
using UnityEngine;

namespace CameraController
{
    [Serializable]
    public class CameraCollisionParameters
    {
        [Tooltip("Wether the camera has a collider")]
        [SerializeField] private bool cameraHasCollider;
        [Tooltip("The parameters of the camera collision")]
        [SerializeField] private CollisionDetection collision;
        [Tooltip("How the camera should act when coming out of a collision")]
        [SerializeField] private DampingParams dampingParams;

        public bool hasCollider => cameraHasCollider;
        public CollisionDetection collisionDetection => collision;
        public DampingParams collisionDamping => dampingParams;

        public CameraCollisionParameters(bool hasCollider, CollisionDetection collisionDetection, DampingParams dampingParams)
        {
            cameraHasCollider = hasCollider;
            collision = collisionDetection;
            this.dampingParams = dampingParams;
        }
    }
}
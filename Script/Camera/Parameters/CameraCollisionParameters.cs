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
        [SerializeField] private SmoothingParameters smoothingParams;

        public bool hasCollider => cameraHasCollider;
        public CollisionDetection collisionDetection => collision;
        public SmoothingParameters collisionSmoothing => smoothingParams;

        public CameraCollisionParameters(bool hasCollider, CollisionDetection collisionDetection, SmoothingParameters smoothingParams)
        {
            cameraHasCollider = hasCollider;
            collision = collisionDetection;
            this.smoothingParams = smoothingParams;
        }
    }
}
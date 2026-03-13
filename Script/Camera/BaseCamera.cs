using UnityEditor;
using UnityEngine;

namespace CameraController
{
    [ExecuteAlways]
    public abstract class BaseCamera : MonoBehaviour
    {
        [Tooltip("The target the camera follow and look at")]
        [SerializeField] protected Transform target;

        protected Transform targetFollower;
        protected Transform cameraTransform;

        private new Camera camera;

        [SerializeField] protected Vector3 offset;

        [Header("Follow")]
        [Tooltip("When the camera movement should be done")]
        [SerializeField] protected FollowMode followMode;
        [Tooltip("How much the camera should lag behind it's target, 0 : always on target position")]
        [SerializeField, Min(0)] protected float followDamping;
        protected Vector3 followSmoothDampVelocity;

        protected virtual void Awake()
        {
            targetFollower = transform;

            camera = Camera.main;
            if (camera)
                cameraTransform = camera.transform;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (target == null)
                    return;

                transform.position = target.position;
                camera.transform.position = transform.rotation * (transform.position + Vector3.Scale(Vector3.one, offset));
                return;
            }
#endif
            if (followMode == FollowMode.Update)
                UpdateCamera();
        }

        protected virtual void FixedUpdate()
        {
            if (followMode == FollowMode.FixedUpdate)
                UpdateCamera();
        }

        protected virtual void LateUpdate()
        {
            if (followMode == FollowMode.LateUpdate)
                UpdateCamera();
        }

        /// <summary>
        /// Update the camera and the target follower position
        /// </summary>
        protected virtual void UpdateCamera()
        {
            if (target == null)
                return;

            UpdateFollowerPosition();
            UpdateCameraPosition();
        }

        /// <summary>
        /// Update the position of the target follower
        /// </summary>
        protected abstract void UpdateFollowerPosition();

        /// <summary>
        /// Update the position of the camera
        /// </summary>
        protected abstract void UpdateCameraPosition();

        /// <summary>
        /// Get the deltatime based on the cuurent followmode
        /// </summary>
        protected float GetDeltaTime() => followMode switch
        {
            FollowMode.FixedUpdate => Time.fixedDeltaTime,
            _ => Time.deltaTime
        };
    }
}
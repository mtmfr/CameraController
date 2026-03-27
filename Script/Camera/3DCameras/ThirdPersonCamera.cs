using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CameraController
{
    public class ThirdPersonCamera : Camera3D
    {
        [Header("Collider")]
        [SerializeField] private CameraCollisionParameters collisionParameters;
        private CollisionDetection collisionDetection => collisionParameters.collisionDetection;
        private DampingParams collisionDamping => collisionParameters.collisionDamping;
        private readonly RaycastHit[] collisionHits = new RaycastHit[2];

        private Vector3 currentOffset;
        private float dampVelocity;

        private void Start()
        {
            currentOffset = offset;
        }

        protected override void UpdateFollowerPosition()
        {
            if (Mathf.Approximately(followDamping, 0f))
            {
                targetFollower.position = target.position;
                return;
            }

            Vector3 targetPos = target.position;
            Vector3 followerPos = targetFollower.position;

            Vector3 posDelta = targetPos - followerPos;

            float followSpeed = posDelta.magnitude / followDamping;

            Vector3 posToReach = targetPos + posDelta.normalized * posDelta.magnitude;

            Vector3 newFollowerPos = Vector3.MoveTowards(followerPos, posToReach, followSpeed * GetDeltaTime());

            targetFollower.position = newFollowerPos;
        }

        protected override void UpdateCameraPosition()
        {
            targetFollower.GetPositionAndRotation(out Vector3 currentPosition, out Quaternion currentRotation);
            Vector3 desiredCameraPos = currentPosition + currentRotation * offset;

            if (!collisionParameters.hasCollider)
            {
                cameraTransform.position = desiredCameraPos;
                return;
            }

            if (IsViewBlocked(desiredCameraPos, out Vector3 blockedPos))
                cameraTransform.position = GetBlockedPosition(currentPosition, currentRotation, cameraTransform.position, blockedPos);
            else GoToDesiredCameraPos(cameraTransform.position, currentRotation, desiredCameraPos);

            MakeCameraLookAtTarget();
        }

        /// <summary>
        /// Send a sphereCast to see if there is an object blocking the view
        /// </summary>
        /// <param name="desiredCameraPos">the position of the camera</param>
        /// <param name="hitPosition">The position where there is an object blocking the view.</param>
        /// <returns>true if an object blocking the view and give blockedAt the position at wich the view is blocked</returns>
        private bool IsViewBlocked(Vector3 desiredCameraPos, out Vector3 hitPosition)
        {
            Vector3 currentPos = targetFollower.position;
            Ray ray = new (currentPos, (desiredCameraPos - currentPos).normalized);

            float castLength = Vector3.Distance(cameraTransform.position, currentPos) + collisionDetection.colliderRadius;

            int blockingObject = Physics.SphereCastNonAlloc(ray, collisionDetection.colliderRadius, collisionHits, castLength, collisionDetection.mask.value, collisionDetection.collisionQuery);

            if (blockingObject > 0)
            {
                for (int objectId = 0; objectId < blockingObject; objectId++)
                {
                    RaycastHit hit = collisionHits[objectId];
                    if (hit.transform == target)
                        continue;

                    hitPosition = hit.point;
                    return true;
                }
            }

            hitPosition = desiredCameraPos;
            return false;
        }

        /// <summary>
        /// Get the position the camera should go at if there is an object blocking the view
        /// </summary>
        /// <param name="position">the position the camera follows</param>
        /// <param name="rotation">the rotation applied to the camera offset</param>
        /// <param name="currentPosition">the current position of the camera</param>
        /// <param name="hitPosition">the position at wich an object bloxking the view was detected</param>
        private Vector3 GetBlockedPosition(Vector3 position, Quaternion rotation, Vector3 currentPosition, Vector3 hitPosition)
        {
            CollisionDetection collisionParams = collisionParameters.collisionDetection;

            float distance = Vector3.Distance(position, hitPosition) - collisionParams.colliderRadius;

            currentOffset.z = Mathf.Sign(offset.z) * distance;

            Vector3 cameraPos = position + rotation * currentOffset;

            //Check if the target is inside the camera
            if (Physics.CheckSphere(cameraPos, collisionParams.colliderRadius))
            {
                return currentPosition;
            }
            else return cameraPos;
        }

        /// <summary>
        /// Move the camera to the desired position
        /// </summary>
        /// <param name="currentCameraPos">the current position of the camera</param>
        /// <param name="rotation">the rotation applied to the camera offset</param>
        /// <param name="desiredCameraPos">the position the camera should be at</param>
        private void GoToDesiredCameraPos(Vector3 currentCameraPos, Quaternion rotation, Vector3 desiredCameraPos)
        {
            float currentOffsetForward = currentOffset.z;

            if (currentOffsetForward - offset.z < 10e-3f)
            {
                currentOffset.z = offset.z;
                cameraTransform.position = desiredCameraPos;
            }
            else
            {
                float deltaForward = offset.z - currentOffsetForward;
                float newForward = collisionDamping.EasingType switch
                {
                    DampingParams.EaseType.EaseIn => currentOffsetForward + currentOffsetForward / collisionDamping.Damping * GetDeltaTime(),
                    DampingParams.EaseType.EaseInThenEaseOut => Mathf.SmoothDamp(currentOffsetForward, offset.z, ref dampVelocity, collisionDamping.Damping, Mathf.Infinity, GetDeltaTime()),
                    _ => currentOffsetForward + deltaForward / collisionDamping.Damping * GetDeltaTime(),
                };
                currentOffset.z = newForward;
                cameraTransform.position = targetFollower.position + rotation * currentOffset;
            }
        }

        private void MakeCameraLookAtTarget()
        {
            Quaternion cameraRotation = Quaternion.LookRotation(target.position - cameraTransform.position);

            cameraTransform.rotation = cameraRotation;
        }
        
        #if UNITY_EDITOR
        [MenuItem("GameObject/CameraController/3rdPersonCamera", false, 1)]
        private static void CreateCamera()
        {
            GameObject toCreate = new("3rdPersonCamera", typeof(ThirdPersonCamera));

            ThirdPersonCamera cam = toCreate.GetComponent<ThirdPersonCamera>();
            cam.offset = new Vector3(0, 0, -10);

            RotationLimits newYawLimits = new(-180, 180, true);
            RotationLimits newPitchLimits = new(-40, 40, false);

            cam.sensitivity = new Vector2(0.7f, 0.7f);

            cam.SetRotationLimits(newYawLimits, newPitchLimits);
            cam.followDamping = 0.1f;

            CollisionDetection detection = new(0.5f, Physics.AllLayers, QueryTriggerInteraction.UseGlobal);
            DampingParams damping = new(0.3f, DampingParams.EaseType.EaseIn);

            CameraCollisionParameters collisionParameters = new(true, detection, damping);
            cam.collisionParameters = collisionParameters;

            SceneView lastview = SceneView.lastActiveSceneView;
            toCreate.transform.position = lastview ? lastview.pivot : Vector3.zero;
            StageUtility.PlaceGameObjectInCurrentStage(toCreate);
            GameObjectUtility.EnsureUniqueNameForSibling(toCreate);

            Undo.RegisterCreatedObjectUndo(toCreate, toCreate.name);
            Selection.activeObject = toCreate;
        }
        #endif
    }
}
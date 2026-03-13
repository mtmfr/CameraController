using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    public class Camera2D : BaseCamera
    {
        [Header("OffsetDirection")]
        [Tooltip("Wether the offset needs to change with the target facing direction")]
        [SerializeField] private bool offsetFollowMovement;
        [SerializeField, Min(0)] private float offsetDamping;
        private float offsetDir = 1f;

        [Header("LookAhead")]
        [SerializeField] private InputActionReference lookAheadInput;
        [SerializeField] private Vector2 maxLookAhead;
        [SerializeField, Min(0)] private float lookAheadDamping;
        private Vector2 lookAheadDir;
        private Vector2 lookAhead;

        private Vector3 currentOffset;

        private void Start()
        {
            currentOffset = offset;
        }

        private void OnEnable()
        {
            if (lookAheadInput != null)
                lookAheadInput.action.performed += SetLookAheadDir;
        }

        private void OnDisable()
        {
            if(lookAheadInput != null)
                lookAheadInput.action.performed -= SetLookAheadDir;
        }

        protected override void UpdateCamera()
        {
            if (target == null)
                return;

            base.UpdateCamera();
            LookAhead();
        }

        protected override void UpdateFollowerPosition()
        {
            Vector3 targetPos = target.position;
            Vector3 followerPos = targetFollower.position;

            Vector3 deltaPos = targetPos - followerPos;

            if (offsetFollowMovement && !Mathf.Approximately(deltaPos.x, 0f))
                offsetDir = Mathf.Sign(deltaPos.x);

            if (!Mathf.Approximately(followDamping, 0f))
            {
                Vector3 movementSpeed = deltaPos / followDamping;

                Vector3 newPos = followerPos + movementSpeed * Time.deltaTime;

                targetFollower.position = newPos;
            }
            else targetFollower.position = targetPos;
        }

        protected override void UpdateCameraPosition()
        {
            Vector3 targetOffset = offset;
            targetOffset.x *= offsetDir;

            Vector3 offsetSpeed = (targetOffset - currentOffset) / offsetDamping;

            if (!float.IsFinite(offsetSpeed.x))
            {
                cameraTransform.position = targetFollower.position + targetOffset;
                return;
            }

            Vector3 newOffset = currentOffset + offsetSpeed * GetDeltaTime();

            currentOffset = newOffset;

            cameraTransform.position = targetFollower.position + currentOffset + (Vector3)lookAhead;
        }

        private void SetLookAheadDir(InputAction.CallbackContext context) => lookAheadDir = context.ReadValue<Vector2>();
        
        private void LookAhead()
        {
            float xLookAhead = lookAhead.x;

            if (!Mathf.Approximately(lookAheadDir.x, 0f))
            {
                float desiredValue = Mathf.Sign(lookAheadDir.x) * maxLookAhead.x;

                xLookAhead = GetLookAhead(xLookAhead, desiredValue, lookAheadDamping);
            }
            else
                xLookAhead = GetLookAhead(xLookAhead, 0, lookAheadDamping);

            float yLookAhead = lookAhead.y;

            if (!Mathf.Approximately(lookAheadDir.y, 0f))
            {
                float desiredValue = Mathf.Sign(lookAheadDir.y) * maxLookAhead.y;
                yLookAhead = GetLookAhead(yLookAhead, desiredValue, lookAheadDamping);
            }
            else
                yLookAhead = GetLookAhead(yLookAhead, 0, lookAheadDamping);

            lookAhead = new Vector2(xLookAhead, yLookAhead);
        }

        private float GetLookAhead(float currentLookAhead, float maxLookAhead, float damping)
        {
            float value = currentLookAhead;

            float changeVelocity = (maxLookAhead - currentLookAhead) / damping;

            float nextValue = value + changeVelocity * GetDeltaTime();

            return float.IsFinite(nextValue) ? nextValue : maxLookAhead;
        }


        [MenuItem("GameObject/CameraController/2DCamera", false, 2)]
        private static void CreateCamera()
        {
            GameObject toCreate = new("2DCamera", typeof(Camera2D));

            Camera2D cam = toCreate.GetComponent<Camera2D>();
            cam.offset = new Vector3(0, 0, -10);
            cam.followDamping = 0f;

            SceneView lastview = SceneView.lastActiveSceneView;
            toCreate.transform.position = lastview ? lastview.pivot : Vector3.zero;
            StageUtility.PlaceGameObjectInCurrentStage(toCreate);
            GameObjectUtility.EnsureUniqueNameForSibling(toCreate);

            Undo.RegisterCreatedObjectUndo(toCreate, toCreate.name);
            Selection.activeObject = toCreate;
        }
    }
}
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
        [SerializeField, Min(0)] private float offsetSmoothingTime;
        [SerializeField] private EaseType offsetEasingType;
        private float offsetFacingDir = 1f;

        [Header("LookAhead")]
        [SerializeField] private Camera2DLookAheadControl lookAheadControl;

        private Vector2 lookAheadInputDir;
        private Vector2 currentLookAhead;

        private Vector3 currentOffset;

        private void Start()
        {
            currentOffset = offset;
        }

        private void OnEnable()
        {
#if (UNITY_EDITOR)
            if (!EditorApplication.isPlaying)
                return;
#endif
            if (lookAheadControl.action != null)
                lookAheadControl.action.performed += SetLookAheadDir;
        }

        private void OnDisable()
        {
#if (UNITY_EDITOR)
            if (!EditorApplication.isPlaying)
                return;
#endif
            if (lookAheadControl.action != null)
                lookAheadControl.action.performed -= SetLookAheadDir;
        }

        protected override void UpdateCamera()
        {
            if (target == null)
                return;

            base.UpdateCamera();
            LookAhead();
            UpdateOffsetDirection();
        }

        protected override void UpdateFollowerPosition()
        {
            Vector3 targetPos = target.position;
            Vector3 followerPos = targetFollower.position;

            Vector3 deltaPos = targetPos - followerPos;

            if (offsetFollowMovement && !Mathf.Approximately(deltaPos.x, 0f))
                offsetFacingDir = Mathf.Sign(deltaPos.x);

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
            UpdateOffsetDirection();

            cameraTransform.position = targetFollower.position + currentOffset + (Vector3)currentLookAhead;
        }

        /// <summary>
        /// Update the direction the offset is looking at when the target facing direction change
        /// </summary>
        private void UpdateOffsetDirection()
        {
            float targetXOffset = offset.x * offsetFacingDir;

            currentOffset.x = Smoothing.SmoothValue(-targetXOffset, targetXOffset, currentOffset.x, offsetSmoothingTime, offsetEasingType, GetDeltaTime());

            //float offsetSpeed = offset.x / offsetSmoothingTime;

            //if (!float.IsFinite(offsetSpeed))
            //{
            //    currentOffset.x = targetXOffset;
            //    return;
            //}

            //currentOffset.x = Mathf.MoveTowards(currentOffset.x, targetXOffset, offsetSpeed * GetDeltaTime());
        }

        private void SetLookAheadDir(InputAction.CallbackContext context) => lookAheadInputDir = context.ReadValue<Vector2>();
        
        private void LookAhead()
        {
            float newXLookAhead = GetNewLookAhead(0, lookAheadInputDir.x, lookAheadControl.maxLookAhead.x, currentLookAhead.x);
            float newYLookAhead = GetNewLookAhead(0, lookAheadInputDir.y, lookAheadControl.maxLookAhead.y, currentLookAhead.y);

            currentLookAhead = new(newXLookAhead, newYLookAhead);
        }

        private float GetNewLookAhead(float startValue, float inputDir, float maxLookAhead, float currentValue)
        {
            if (Mathf.Approximately(inputDir, 0))
            {
                return Smoothing.SmoothValue(maxLookAhead * Mathf.Sign(currentValue), 0, currentValue, lookAheadControl.smoothingTime, lookAheadControl.easingType, GetDeltaTime());
            }

            float targetValue = inputDir * maxLookAhead;

            if (Mathf.Sign(targetValue) != Mathf.Sign(currentValue) && !Mathf.Approximately(currentValue, 0))
                return Smoothing.SmoothValue(-targetValue, targetValue, currentValue, lookAheadControl.smoothingTime, lookAheadControl.easingType, GetDeltaTime());

            return Smoothing.SmoothValue(startValue, targetValue, currentValue, lookAheadControl.smoothingTime, lookAheadControl.easingType, GetDeltaTime());
        }


        [MenuItem("GameObject/CameraController/2DCamera", false, 2)]
        private static void CreateCamera()
        {
            GameObject toCreate = new("2DCamera", typeof(Camera2D));

            Camera2D cam = toCreate.GetComponent<Camera2D>();
            cam.offset = new Vector3(0, 0, -10);
            cam.followDamping = 0f;

            string defaultInputPath = "Assets/CameraController/CameraController_DefaultInput.inputactions";

            if (AssetDatabase.AssetPathExists(defaultInputPath))
            {
                InputActionAsset actionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(defaultInputPath);
                InputAction action = actionAsset.FindAction("LookAhead");

                InputActionReference actionReference = InputActionReference.Create(action);
                cam.lookAheadControl = Camera2DLookAheadControl.Create(true, Vector2.one, 0.3f, EaseType.Linear, actionReference);
            }
            else cam.lookAheadControl = Camera2DLookAheadControl.Create(true, Vector2.zero, 0.3f, EaseType.Linear);
            
            SceneView lastview = SceneView.lastActiveSceneView;
            toCreate.transform.position = lastview ? lastview.pivot : Vector3.zero;
            StageUtility.PlaceGameObjectInCurrentStage(toCreate);
            GameObjectUtility.EnsureUniqueNameForSibling(toCreate);

            Undo.RegisterCreatedObjectUndo(toCreate, toCreate.name);
            Selection.activeObject = toCreate;
        }
    }
}
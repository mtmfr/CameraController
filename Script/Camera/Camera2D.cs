using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    public class Camera2D : BaseCamera
    {
        [Header("Offset")]
        [Tooltip("Wether the offset needs to change with the target facing direction")]
        [SerializeField] private bool offsetFollowMovement;
        [Tooltip("How long it takes for the offset to change")]
        [SerializeField] SmoothingParameters offsetSmoothingParams;

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
            UpdateLookAhead();
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

            currentOffset.x = Smoothing.SmoothValue(-targetXOffset, targetXOffset, currentOffset.x, offsetSmoothingParams.smoothingTime, offsetSmoothingParams.EasingType, GetDeltaTime());
        }

        /// <summary>
        /// Set the value of the look ahead input
        /// </summary>
        /// <param name="context"></param>
        private void SetLookAheadDir(InputAction.CallbackContext context) => lookAheadInputDir = context.ReadValue<Vector2>();
        
        /// <summary>
        /// Update the current look ahead
        /// </summary>
        private void UpdateLookAhead()
        {
            float newXLookAhead = GetNewLookAhead(0, lookAheadInputDir.x, lookAheadControl.maxLookAhead.x, currentLookAhead.x);
            float newYLookAhead = GetNewLookAhead(0, lookAheadInputDir.y, lookAheadControl.maxLookAhead.y, currentLookAhead.y);

            currentLookAhead = new(newXLookAhead, newYLookAhead);
        }

        /// <summary>
        /// Get the new lookahead of the given value.
        /// </summary>
        /// <param name="startValue">the default position if no input are given.</param>
        /// <param name="inputDir">the direction of the input. Should be '-1' or '1'.</param>
        /// <param name="maxLookAhead">the maximum value the lookahead can have. Is the limit for both the positive and negative.</param>
        /// <param name="currentValue">The current look ahead.</param>
        /// <returns>The new value of the look ahead</returns>
        private float GetNewLookAhead(float startValue, float inputDir, float maxLookAhead, float currentValue)
        {
            //Go to default value if there are no input
            if (Mathf.Approximately(inputDir, 0))
                return Smoothing.SmoothValue(maxLookAhead * Mathf.Sign(currentValue), 0, currentValue, lookAheadControl.smoothingTime, lookAheadControl.easingType, GetDeltaTime());

            float targetValue = inputDir * maxLookAhead;
            //If the target value is the opposite of the current value
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
                cam.lookAheadControl = new(true, Vector2.one, 0.3f, EaseType.Linear, actionReference);
            }
            else cam.lookAheadControl = new(true, Vector2.zero, 0.3f, EaseType.Linear);
            
            SceneView lastview = SceneView.lastActiveSceneView;
            toCreate.transform.position = lastview ? lastview.pivot : Vector3.zero;
            StageUtility.PlaceGameObjectInCurrentStage(toCreate);
            GameObjectUtility.EnsureUniqueNameForSibling(toCreate);

            Undo.RegisterCreatedObjectUndo(toCreate, toCreate.name);
            Selection.activeObject = toCreate;
        }
    }
}
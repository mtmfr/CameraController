using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    public class FirstPersonCamera : Camera3D
    {
        protected override void UpdateFollowerPosition()
        {
            targetFollower.position = target.position;
        }

        protected override void UpdateCameraPosition()
        {
            cameraTransform.SetPositionAndRotation(GetCameraPosition(), targetFollower.rotation);
        }

        private Vector3 GetCameraPosition() => targetFollower.position + offset;
        
        #if UNITY_EDITOR
        [MenuItem("GameObject/CameraController/1stPersonCamera",false, 0)]
        private static void CreateCamera()
        {
            GameObject toCreate = new("1stPersonCamera", typeof(FirstPersonCamera));

            FirstPersonCamera cam = toCreate.GetComponent<FirstPersonCamera>();
            cam.offset = Vector3.up;

            cam.followDamping = 0;

            string defaultInputPath = "Assets/CameraController/CameraController_DefaultInput.inputactions";
            InputActionReference actionReference = null;

            if (AssetDatabase.AssetPathExists(defaultInputPath))
            {
                InputActionAsset actionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(defaultInputPath);

                InputAction action = actionAsset.FindAction("Look");

                actionReference = InputActionReference.Create(action);
            }

            cam.SetInputControl(CameraInput.CreateInputParameter(true, 0.7f, 0.7f, actionReference));

            RotationLimits newYawLimits = new(-180, 180, true);
            RotationLimits newPitchLimits = new(-40, 40, false);

            cam.SetRotationLimits(newYawLimits, newPitchLimits);

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
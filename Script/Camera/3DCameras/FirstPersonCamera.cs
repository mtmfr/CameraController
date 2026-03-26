using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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

            cam.sensitivity = new Vector2(0.7f, 0.7f);

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
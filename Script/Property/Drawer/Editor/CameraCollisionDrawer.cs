using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CameraController
{
    [CustomPropertyDrawer(typeof(CameraCollisionParameters))]
    public class CameraCollisionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            PropertyField hasColliderProperty = new(property.FindPropertyRelative("cameraHasCollider"), "cameraCollider");

            root.Add(hasColliderProperty);

            PropertyField collisionDetectionProperty = new(property.FindPropertyRelative("collision"), "collisionDetection");
            PropertyField dampingProperty = new(property.FindPropertyRelative("dampingParams"), "collisionDamping");

            root.Add(collisionDetectionProperty);
            root.Add(dampingProperty);

            hasColliderProperty.RegisterValueChangeCallback(callback =>
            {
                bool value = callback.changedProperty.boolValue;

                DisplayStyle displayStyle = value ? DisplayStyle.Flex : DisplayStyle.None;

                collisionDetectionProperty.style.display = displayStyle;
                dampingProperty.style.display = displayStyle;
            });

            return root;
        }
    }
}
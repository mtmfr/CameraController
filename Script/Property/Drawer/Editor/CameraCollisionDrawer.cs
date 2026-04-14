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
            SerializedProperty hasColliderProperty = property.FindPropertyRelative("cameraHasCollider");
            Toggle hasColliderToggle = new("Has Collider");
            hasColliderToggle.BindProperty(hasColliderProperty);
            root.Add(hasColliderToggle);


            SerializedProperty collisionDetectionProperty = property.FindPropertyRelative("collision");
            Label collisionDetectionLabel = new(collisionDetectionProperty.displayName);

            Foldout collisionDetectionFoldout = new();
            collisionDetectionFoldout.text = collisionDetectionLabel.text;
            root.Add(collisionDetectionFoldout);

            SerializedProperty collisionRadiusProperty = collisionDetectionProperty.FindPropertyRelative("collisionRadius");
            FloatField collisionRadiusField = new("Collider Radius");
            collisionRadiusField.BindProperty(collisionRadiusProperty);
            collisionDetectionFoldout.Add(collisionRadiusField);

            SerializedProperty collisionMaskProperty = collisionDetectionProperty.FindPropertyRelative("collisionMask");
            LayerMaskField collisionMaskField = new(collisionMaskProperty.displayName);
            collisionMaskField.BindProperty(collisionMaskProperty);
            collisionDetectionFoldout.Add(collisionMaskField);

            SerializedProperty queryProperty = collisionDetectionProperty.FindPropertyRelative("queryParameters");
            EnumField queryField = new("Collision Query");
            queryField.BindProperty(queryProperty);
            collisionDetectionFoldout.Add(queryField);

            PropertyField smoothingField = new(property.FindPropertyRelative("smoothingParams"), "Collision Smoothing");
            root.Add(smoothingField);

            hasColliderToggle.RegisterValueChangedCallback(callback =>
            {
                DisplayStyle displayStyle = callback.newValue ? DisplayStyle.Flex : DisplayStyle.None;

                collisionDetectionFoldout.style.display = displayStyle;
                smoothingField.style.display = displayStyle;
            });

            return root;
        }
    }
}
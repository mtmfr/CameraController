using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace CameraController
{
    [CustomPropertyDrawer(typeof(Camera2DLookAheadControl))]
    public class Camera2DLookAheadControlDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();

            SerializedProperty hasInputProperty = property.FindPropertyRelative("canLookAhead");
            Toggle hasInputToggle = new("Has Look Ahead");
            hasInputToggle.BindProperty(hasInputProperty);
            root.Add(hasInputToggle);

            SerializedProperty inputRef = property.FindPropertyRelative("input");
            ObjectField inputField = new(inputRef.displayName)
            {
                allowSceneObjects = false,
                objectType = typeof(InputActionReference)
            };
            inputField.BindProperty(inputRef);
            root.Add(inputField);

            SerializedProperty lookAheadProperty = property.FindPropertyRelative("lookAhead");
            Vector2Field lookAheadField = new(lookAheadProperty.displayName);
            lookAheadField.BindProperty(lookAheadProperty);
            root.Add(lookAheadField);

            PropertyField smoothParamField = new(property.FindPropertyRelative("lookAheadSmoothing"));
            root.Add(smoothParamField);

            hasInputToggle.RegisterValueChangedCallback(callback =>
            {
                DisplayStyle displayStyle = callback.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                inputField.style.display = displayStyle;
                lookAheadField.style.display = displayStyle;
                smoothParamField.style.display = displayStyle;
            });


            return root;
        }
    }
}


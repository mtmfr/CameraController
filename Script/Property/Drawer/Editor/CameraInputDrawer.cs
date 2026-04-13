using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace CameraController
{
    [CustomPropertyDrawer(typeof(CameraInput))]
    public class CameraInputDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();

            SerializedProperty canBeControlled = property.FindPropertyRelative("canControlCamera");
            Toggle toggleField = new(canBeControlled.displayName);
            root.Add(toggleField);
            toggleField.BindProperty(canBeControlled);


            SerializedProperty inputReference = property.FindPropertyRelative("controlInput");
            ObjectField inputField = new(inputReference.displayName);
            inputField.objectType = typeof(InputActionReference);

            inputField.BindProperty(inputReference);
            root.Add(inputField);

            SerializedProperty sensitivity = property.FindPropertyRelative("sensitivity");
            Vector2Field sensitivityField = new(sensitivity.displayName);
            root.Add(sensitivityField);
            sensitivityField.BindProperty(sensitivity);

            sensitivityField.RegisterValueChangedCallback(callback =>
            {
                Vector2 newValue = callback.newValue;

                float minAllowedValue = 0.1f;

                float x = newValue.x < minAllowedValue ? minAllowedValue : newValue.x;
                float y = newValue.y < minAllowedValue ? minAllowedValue : newValue.y;

                sensitivityField.SetValueWithoutNotify(new(x, y));
            });

            toggleField.RegisterValueChangedCallback(callback =>
            {
                DisplayStyle displayStyle = callback.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                inputField.style.display = displayStyle;
                sensitivityField.style.display = displayStyle;
            });
            
            return root;
        }
    }
}
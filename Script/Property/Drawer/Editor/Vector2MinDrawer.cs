using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraController
{
    [CustomPropertyDrawer(typeof(Vector2MinAttribute))]
    public class Vector2MinDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Vector2MinAttribute minAttibute = attribute as Vector2MinAttribute;

            Vector2Field vectorField = new(property.displayName);
            vectorField.BindProperty(property);

            vectorField.RegisterValueChangedCallback(callback =>
            {
                Vector2 newValue = callback.newValue;

                newValue.x = newValue.x > minAttibute.yMin ? newValue.x : minAttibute.xMin;
                newValue.y = newValue.y > minAttibute.yMin ? newValue.y : minAttibute.yMin;

            });

            return vectorField;
        }
    }
}
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CameraController
{
    [CustomPropertyDrawer(typeof(SmoothingParameters))]
    public class SmoothingParametersDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();

            SerializedProperty smoothTimeProperty = property.FindPropertyRelative("smoothTime");
            FloatField timeField = new("Smoothing Time");
            timeField.BindProperty(smoothTimeProperty);
            root.Add(timeField);

            SerializedProperty easeTypeProperty = property.FindPropertyRelative("easeType");
            EnumField enumField = new("Easing Type");
            enumField.BindProperty(easeTypeProperty);
            timeField.Add(enumField);
            enumField.style.width = new(Length.Percent(50f));

            timeField.RegisterValueChangedCallback(callback =>
            {
                if (callback.newValue < 0)
                    timeField.SetValueWithoutNotify(0f);
            });

            return root;
        }
    }
}


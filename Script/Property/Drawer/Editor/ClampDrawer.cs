using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraController
{
    [CustomPropertyDrawer(typeof(ClampAttribute))]
    public class ClampDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ClampAttribute clampAttribute = attribute as ClampAttribute;

            Type fieldType = fieldInfo.FieldType;

            if (fieldType == typeof(float))
            {
                FloatField floatField = new(property.displayName);
                floatField.BindProperty(property);

                floatField.SetValueWithoutNotify(property.floatValue);

                floatField.RegisterValueChangedCallback(callback =>
                {
                    float value = Mathf.Clamp(callback.newValue, clampAttribute.min, clampAttribute.max);
                    floatField.value = value;
                });

                return floatField;
            }
            else if (fieldType == typeof(int))
            {
                IntegerField intField = new(property.displayName);
                intField.BindProperty(property);

                intField.SetValueWithoutNotify(property.intValue);

                intField.RegisterValueChangedCallback(callback =>
                {
                    int value = callback.newValue;
                    intField.value = value;
                });
                return intField;
            }

            return null;
        }
    }
}
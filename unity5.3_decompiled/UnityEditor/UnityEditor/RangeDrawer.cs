namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(RangeAttribute))]
    internal sealed class RangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RangeAttribute attribute = (RangeAttribute) base.attribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(position, property, attribute.min, attribute.max, label);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(position, property, (int) attribute.min, (int) attribute.max, label);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
            }
        }
    }
}


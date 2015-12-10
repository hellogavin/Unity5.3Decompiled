namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ColorUsageAttribute))]
    internal sealed class ColorUsageDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ColorUsageAttribute attribute = (ColorUsageAttribute) base.attribute;
            ColorPickerHDRConfig hdrConfig = ColorPickerHDRConfig.Temp(attribute.minBrightness, attribute.maxBrightness, attribute.minExposureValue, attribute.maxExposureValue);
            EditorGUI.BeginChangeCheck();
            Color color = EditorGUI.ColorField(position, label, property.colorValue, true, attribute.showAlpha, attribute.hdr, hdrConfig);
            if (EditorGUI.EndChangeCheck())
            {
                property.colorValue = color;
            }
        }
    }
}


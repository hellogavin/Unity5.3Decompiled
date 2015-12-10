namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(TextAreaAttribute))]
    internal sealed class TextAreaDrawer : PropertyDrawer
    {
        private const int kLineHeight = 13;
        private Vector2 m_ScrollPosition = new Vector2();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            TextAreaAttribute attribute = base.attribute as TextAreaAttribute;
            string stringValue = property.stringValue;
            int num2 = Mathf.Clamp(Mathf.CeilToInt(EditorStyles.textArea.CalcHeight(GUIContent.Temp(stringValue), EditorGUIUtility.contextWidth) / 13f), attribute.minLines, attribute.maxLines);
            return (32f + ((num2 - 1) * 13));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                label = EditorGUI.BeginProperty(position, label, property);
                Rect labelPosition = position;
                labelPosition.height = 16f;
                position.yMin += labelPosition.height;
                EditorGUI.HandlePrefixLabel(position, labelPosition, label);
                EditorGUI.BeginChangeCheck();
                string str = EditorGUI.ScrollableTextAreaInternal(position, property.stringValue, ref this.m_ScrollPosition, EditorStyles.textArea);
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = str;
                }
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use TextAreaDrawer with string.");
            }
        }
    }
}


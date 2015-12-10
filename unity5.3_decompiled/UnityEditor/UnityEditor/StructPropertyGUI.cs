namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class StructPropertyGUI
    {
        private static void DoChildren(Rect position, SerializedProperty property)
        {
            float depth = property.depth;
            position.height = 16f;
            EditorGUI.indentLevel++;
            SerializedProperty property2 = property.Copy();
            property2.NextVisible(true);
            while (property2.depth == (depth + 1f))
            {
                EditorGUI.PropertyField(position, property2);
                position.y += 16f;
                property2.NextVisible(false);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        internal static void GenericStruct(Rect position, SerializedProperty property)
        {
            GUI.Label(EditorGUI.IndentedRect(position), property.displayName, EditorStyles.label);
            position.y += 16f;
            DoChildren(position, property);
        }

        internal static void JointSpring(Rect position, SerializedProperty property)
        {
            GenericStruct(position, property);
        }

        internal static void WheelFrictionCurve(Rect position, SerializedProperty property)
        {
            GenericStruct(position, property);
        }
    }
}


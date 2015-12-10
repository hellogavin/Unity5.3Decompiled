namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class StructPropertyGUILayout
    {
        internal static void GenericStruct(SerializedProperty property, params GUILayoutOption[] options)
        {
            float minHeight = 16f + (16f * GetChildrenCount(property));
            StructPropertyGUI.GenericStruct(GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, minHeight, minHeight, EditorStyles.layerMaskField, options), property);
        }

        internal static int GetChildrenCount(SerializedProperty property)
        {
            int depth = property.depth;
            SerializedProperty property2 = property.Copy();
            property2.NextVisible(true);
            int num2 = 0;
            while (property2.depth == (depth + 1))
            {
                num2++;
                property2.NextVisible(false);
            }
            return num2;
        }

        internal static void JointSpring(SerializedProperty property, params GUILayoutOption[] options)
        {
            GenericStruct(property, options);
        }

        internal static void WheelFrictionCurve(SerializedProperty property, params GUILayoutOption[] options)
        {
            GenericStruct(property, options);
        }
    }
}


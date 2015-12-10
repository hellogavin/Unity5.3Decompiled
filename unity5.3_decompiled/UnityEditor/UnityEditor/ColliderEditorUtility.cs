namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ColliderEditorUtility
    {
        private const float k_EditColliderbuttonHeight = 22f;
        private const float k_EditColliderbuttonWidth = 22f;
        private const float k_SpaceBetweenLabelAndButton = 5f;
        private static GUIStyle s_EditColliderButtonStyle;

        public static bool InspectorEditButtonGUI(bool editing)
        {
            if (s_EditColliderButtonStyle == null)
            {
                s_EditColliderButtonStyle = new GUIStyle("Button");
                s_EditColliderButtonStyle.padding = new RectOffset(0, 0, 0, 0);
                s_EditColliderButtonStyle.margin = new RectOffset(0, 0, 0, 0);
            }
            EditorGUI.BeginChangeCheck();
            Rect rect = EditorGUILayout.GetControlRect(true, 22f, new GUILayoutOption[0]);
            Rect position = new Rect(rect.xMin + EditorGUIUtility.labelWidth, rect.yMin, 22f, 22f);
            GUIContent content = new GUIContent("Edit Collider");
            Vector2 vector = GUI.skin.label.CalcSize(content);
            Rect rect3 = new Rect(position.xMax + 5f, rect.yMin + ((rect.height - vector.y) * 0.5f), vector.x, rect.height);
            GUILayout.Space(2f);
            bool flag = GUI.Toggle(position, editing, EditorGUIUtility.IconContent("EditCollider"), s_EditColliderButtonStyle);
            GUI.Label(rect3, "Edit Collider");
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
            return flag;
        }
    }
}


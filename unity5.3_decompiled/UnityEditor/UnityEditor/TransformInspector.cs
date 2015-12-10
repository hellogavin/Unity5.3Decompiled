namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    internal class TransformInspector : Editor
    {
        private SerializedProperty m_Position;
        private TransformRotationGUI m_RotationGUI;
        private SerializedProperty m_Scale;
        private static Contents s_Contents;

        private void Inspector3D()
        {
            EditorGUILayout.PropertyField(this.m_Position, s_Contents.positionContent, new GUILayoutOption[0]);
            this.m_RotationGUI.RotationField();
            EditorGUILayout.PropertyField(this.m_Scale, s_Contents.scaleContent, new GUILayoutOption[0]);
        }

        public void OnEnable()
        {
            this.m_Position = base.serializedObject.FindProperty("m_LocalPosition");
            this.m_Scale = base.serializedObject.FindProperty("m_LocalScale");
            if (this.m_RotationGUI == null)
            {
                this.m_RotationGUI = new TransformRotationGUI();
            }
            this.m_RotationGUI.OnEnable(base.serializedObject.FindProperty("m_LocalRotation"), new GUIContent(LocalizationDatabase.GetLocalizedString("Rotation")));
        }

        public override void OnInspectorGUI()
        {
            if (s_Contents == null)
            {
                s_Contents = new Contents();
            }
            if (!EditorGUIUtility.wideMode)
            {
                EditorGUIUtility.wideMode = true;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212f;
            }
            base.serializedObject.Update();
            this.Inspector3D();
            Transform target = this.target as Transform;
            Vector3 position = target.position;
            if (((Mathf.Abs(position.x) > 100000f) || (Mathf.Abs(position.y) > 100000f)) || (Mathf.Abs(position.z) > 100000f))
            {
                EditorGUILayout.HelpBox(s_Contents.floatingPointWarning, MessageType.Warning);
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private class Contents
        {
            public string floatingPointWarning = LocalizationDatabase.GetLocalizedString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");
            public GUIContent positionContent = new GUIContent(LocalizationDatabase.GetLocalizedString("Position"), LocalizationDatabase.GetLocalizedString("The local position of this Game Object relative to the parent."));
            public GUIContent scaleContent = new GUIContent(LocalizationDatabase.GetLocalizedString("Scale"), LocalizationDatabase.GetLocalizedString("The local scaling of this Game Object relative to the parent."));
            public GUIContent[] subLabels = new GUIContent[] { new GUIContent("X"), new GUIContent("Y") };
        }
    }
}


namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(RenderSettings))]
    internal class FogEditor : Editor
    {
        private const string kShowEditorKey = "ShowFogEditorFoldout";
        protected SerializedProperty m_Fog;
        protected SerializedProperty m_FogColor;
        protected SerializedProperty m_FogDensity;
        protected SerializedProperty m_FogMode;
        protected SerializedProperty m_LinearFogEnd;
        protected SerializedProperty m_LinearFogStart;
        private bool m_ShowEditor;

        public virtual void OnDisable()
        {
            SessionState.SetBool("ShowFogEditorFoldout", this.m_ShowEditor);
        }

        public virtual void OnEnable()
        {
            this.m_Fog = base.serializedObject.FindProperty("m_Fog");
            this.m_FogColor = base.serializedObject.FindProperty("m_FogColor");
            this.m_FogMode = base.serializedObject.FindProperty("m_FogMode");
            this.m_FogDensity = base.serializedObject.FindProperty("m_FogDensity");
            this.m_LinearFogStart = base.serializedObject.FindProperty("m_LinearFogStart");
            this.m_LinearFogEnd = base.serializedObject.FindProperty("m_LinearFogEnd");
            this.m_ShowEditor = SessionState.GetBool("ShowFogEditorFoldout", false);
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.m_ShowEditor = EditorGUILayout.ToggleTitlebar(this.m_ShowEditor, Styles.fogHeader, this.m_Fog);
            if (this.m_ShowEditor)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!this.m_Fog.boolValue);
                EditorGUILayout.PropertyField(this.m_FogColor, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_FogMode, new GUILayoutOption[0]);
                EditorGUI.indentLevel++;
                if (this.m_FogMode.intValue != 1)
                {
                    EditorGUILayout.PropertyField(this.m_FogDensity, Styles.fogDensity, new GUILayoutOption[0]);
                }
                else
                {
                    EditorGUILayout.PropertyField(this.m_LinearFogStart, Styles.fogLinearStart, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LinearFogEnd, Styles.fogLinearEnd, new GUILayoutOption[0]);
                }
                EditorGUI.indentLevel--;
                if (SceneView.IsUsingDeferredRenderingPath())
                {
                    EditorGUILayout.HelpBox(Styles.fogWarning.text, MessageType.Info);
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        internal class Styles
        {
            public static readonly GUIContent fogDensity = EditorGUIUtility.TextContent("Density");
            public static readonly GUIContent fogHeader = EditorGUIUtility.TextContent("Fog");
            public static readonly GUIContent fogLinearEnd = EditorGUIUtility.TextContent("End");
            public static readonly GUIContent fogLinearStart = EditorGUIUtility.TextContent("Start");
            public static readonly GUIContent fogWarning = EditorGUIUtility.TextContent("Fog does not affect opaque objects in Deferred Shading. Use Global Fog image effect.");
        }
    }
}


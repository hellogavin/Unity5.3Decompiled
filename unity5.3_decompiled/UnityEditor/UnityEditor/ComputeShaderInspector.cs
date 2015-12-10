namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(ComputeShader))]
    internal class ComputeShaderInspector : Editor
    {
        private const float kSpace = 5f;
        private Vector2 m_ScrollPosition = Vector2.zero;

        public virtual void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            ComputeShader target = this.target as ComputeShader;
            if (target != null)
            {
                GUI.enabled = true;
                EditorGUI.indentLevel = 0;
                this.ShowDebuggingData(target);
                this.ShowShaderErrors(target);
            }
        }

        private void ShowDebuggingData(ComputeShader cs)
        {
            GUILayout.Space(5f);
            GUILayout.Label("Compiled code:", EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel("All variants", EditorStyles.miniButton);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            if (GUILayout.Button(Styles.showAll, EditorStyles.miniButton, options))
            {
                ShaderUtil.OpenCompiledComputeShader(cs, true);
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowShaderErrors(ComputeShader s)
        {
            if (ShaderUtil.GetComputeShaderErrorCount(s) >= 1)
            {
                ShaderInspector.ShaderErrorListUI(s, ShaderUtil.GetComputeShaderErrors(s), ref this.m_ScrollPosition);
            }
        }

        internal class Styles
        {
            public static GUIContent showAll = EditorGUIUtility.TextContent("Show code");
        }
    }
}


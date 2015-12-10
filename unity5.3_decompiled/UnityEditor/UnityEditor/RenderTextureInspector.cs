namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(RenderTexture))]
    internal class RenderTextureInspector : TextureInspector
    {
        private static readonly GUIContent[] kRenderTextureAntiAliasing = new GUIContent[] { new GUIContent("None"), new GUIContent("2 samples"), new GUIContent("4 samples"), new GUIContent("8 samples") };
        private static readonly int[] kRenderTextureAntiAliasingValues = new int[] { 1, 2, 4, 8 };
        private SerializedProperty m_AntiAliasing;
        private SerializedProperty m_ColorFormat;
        private SerializedProperty m_DepthFormat;
        private SerializedProperty m_Height;
        private SerializedProperty m_Width;

        public override string GetInfoString()
        {
            RenderTexture target = this.target as RenderTexture;
            string str = target.width + "x" + target.height;
            if (!target.isPowerOfTwo)
            {
                str = str + "(NPOT)";
            }
            return ((str + "  " + target.format) + "  " + EditorUtility.FormatBytes(TextureUtil.GetRuntimeMemorySize(target)));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Width = base.serializedObject.FindProperty("m_Width");
            this.m_Height = base.serializedObject.FindProperty("m_Height");
            this.m_AntiAliasing = base.serializedObject.FindProperty("m_AntiAliasing");
            this.m_ColorFormat = base.serializedObject.FindProperty("m_ColorFormat");
            this.m_DepthFormat = base.serializedObject.FindProperty("m_DepthFormat");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            GUI.changed = false;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel("Size", EditorStyles.popup);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
            EditorGUILayout.PropertyField(this.m_Width, GUIContent.none, options);
            GUILayout.Label("x", new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
            EditorGUILayout.PropertyField(this.m_Height, GUIContent.none, optionArray2);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.IntPopup(this.m_AntiAliasing, kRenderTextureAntiAliasing, kRenderTextureAntiAliasingValues, EditorGUIUtility.TempContent("Anti-Aliasing"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ColorFormat, EditorGUIUtility.TempContent("Color Format"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_DepthFormat, EditorGUIUtility.TempContent("Depth Buffer"), new GUILayoutOption[0]);
            RenderTexture target = this.target as RenderTexture;
            if (GUI.changed && (target != null))
            {
                target.Release();
            }
            base.isInspectorDirty = true;
            EditorGUILayout.Space();
            base.DoWrapModePopup();
            base.DoFilterModePopup();
            EditorGUI.BeginDisabledGroup(this.RenderTextureHasDepth());
            base.DoAnisoLevelSlider();
            EditorGUI.EndDisabledGroup();
            if (this.RenderTextureHasDepth())
            {
                base.m_Aniso.intValue = 0;
                EditorGUILayout.HelpBox("RenderTextures with depth must have an Aniso Level of 0.", MessageType.Info);
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private bool RenderTextureHasDepth()
        {
            return (TextureUtil.IsDepthRTFormat((RenderTextureFormat) this.m_ColorFormat.enumValueIndex) || (this.m_DepthFormat.enumValueIndex != 0));
        }
    }
}


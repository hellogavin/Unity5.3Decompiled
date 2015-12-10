namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [CustomEditor(typeof(GraphicsSettings))]
    internal class GraphicsSettingsInspector : Editor
    {
        protected SerializedProperty m_AlwaysIncludedShaders;
        protected BuiltinShaderSettings m_Deferred;
        protected BuiltinShaderSettings m_DeferredReflections;
        protected SerializedProperty m_FogKeepExp;
        protected SerializedProperty m_FogKeepExp2;
        protected SerializedProperty m_FogKeepLinear;
        protected SerializedProperty m_FogStripping;
        protected BuiltinShaderSettings m_LegacyDeferred;
        protected SerializedProperty m_LightmapKeepDirCombined;
        protected SerializedProperty m_LightmapKeepDirSeparate;
        protected SerializedProperty m_LightmapKeepDynamicDirCombined;
        protected SerializedProperty m_LightmapKeepDynamicDirSeparate;
        protected SerializedProperty m_LightmapKeepDynamicPlain;
        protected SerializedProperty m_LightmapKeepPlain;
        protected SerializedProperty m_LightmapStripping;
        protected SerializedProperty m_PreloadedShaders;

        private void FogStrippingGUI(out bool calcFogStripping)
        {
            calcFogStripping = false;
            EditorGUILayout.PropertyField(this.m_FogStripping, Styles.fogModes, new GUILayoutOption[0]);
            if (this.m_FogStripping.intValue != 0)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_FogKeepLinear, Styles.fogLinear, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_FogKeepExp, Styles.fogExp, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_FogKeepExp2, Styles.fogExp2, new GUILayoutOption[0]);
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUILayout.PrefixLabel(GUIContent.Temp(" "), EditorStyles.miniButton);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                if (GUILayout.Button(Styles.fogFromScene, EditorStyles.miniButton, options))
                {
                    calcFogStripping = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }

        private void LightmapStrippingGUI(out bool calcLightmapStripping)
        {
            calcLightmapStripping = false;
            EditorGUILayout.PropertyField(this.m_LightmapStripping, Styles.lightmapModes, new GUILayoutOption[0]);
            if (this.m_LightmapStripping.intValue != 0)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_LightmapKeepPlain, Styles.lightmapPlain, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_LightmapKeepDirCombined, Styles.lightmapDirCombined, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_LightmapKeepDirSeparate, Styles.lightmapDirSeparate, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_LightmapKeepDynamicPlain, Styles.lightmapDynamicPlain, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_LightmapKeepDynamicDirCombined, Styles.lightmapDynamicDirCombined, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_LightmapKeepDynamicDirSeparate, Styles.lightmapDynamicDirSeparate, new GUILayoutOption[0]);
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUILayout.PrefixLabel(GUIContent.Temp(" "), EditorStyles.miniButton);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                if (GUILayout.Button(Styles.lightmapFromScene, EditorStyles.miniButton, options))
                {
                    calcLightmapStripping = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }

        public virtual void OnEnable()
        {
            this.m_Deferred = new BuiltinShaderSettings(LocalizationDatabase.GetLocalizedString("Deferred|Shader settings for Deferred Shading"), "m_Deferred", base.serializedObject);
            this.m_DeferredReflections = new BuiltinShaderSettings(LocalizationDatabase.GetLocalizedString("Deferred Reflections|Shader settings for deferred reflections."), "m_DeferredReflections", base.serializedObject);
            this.m_LegacyDeferred = new BuiltinShaderSettings(LocalizationDatabase.GetLocalizedString("Legacy Deferred|Shader settings for Legacy (light prepass) Deferred Lighting"), "m_LegacyDeferred", base.serializedObject);
            this.m_AlwaysIncludedShaders = base.serializedObject.FindProperty("m_AlwaysIncludedShaders");
            this.m_AlwaysIncludedShaders.isExpanded = true;
            this.m_PreloadedShaders = base.serializedObject.FindProperty("m_PreloadedShaders");
            this.m_PreloadedShaders.isExpanded = true;
            this.m_LightmapStripping = base.serializedObject.FindProperty("m_LightmapStripping");
            this.m_LightmapKeepPlain = base.serializedObject.FindProperty("m_LightmapKeepPlain");
            this.m_LightmapKeepDirCombined = base.serializedObject.FindProperty("m_LightmapKeepDirCombined");
            this.m_LightmapKeepDirSeparate = base.serializedObject.FindProperty("m_LightmapKeepDirSeparate");
            this.m_LightmapKeepDynamicPlain = base.serializedObject.FindProperty("m_LightmapKeepDynamicPlain");
            this.m_LightmapKeepDynamicDirCombined = base.serializedObject.FindProperty("m_LightmapKeepDynamicDirCombined");
            this.m_LightmapKeepDynamicDirSeparate = base.serializedObject.FindProperty("m_LightmapKeepDynamicDirSeparate");
            this.m_FogStripping = base.serializedObject.FindProperty("m_FogStripping");
            this.m_FogKeepLinear = base.serializedObject.FindProperty("m_FogKeepLinear");
            this.m_FogKeepExp = base.serializedObject.FindProperty("m_FogKeepExp");
            this.m_FogKeepExp2 = base.serializedObject.FindProperty("m_FogKeepExp2");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            bool calcLightmapStripping = false;
            bool calcFogStripping = false;
            GUILayout.Label(Styles.builtinSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_Deferred.DoGUI();
            EditorGUI.BeginChangeCheck();
            this.m_DeferredReflections.DoGUI();
            if (EditorGUI.EndChangeCheck())
            {
                ShaderUtil.ReloadAllShaders();
            }
            this.m_LegacyDeferred.DoGUI();
            EditorGUILayout.PropertyField(this.m_AlwaysIncludedShaders, true, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUILayout.Label(Styles.shaderStrippingSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.LightmapStrippingGUI(out calcLightmapStripping);
            this.FogStrippingGUI(out calcFogStripping);
            this.ShaderPreloadGUI();
            base.serializedObject.ApplyModifiedProperties();
            if (calcLightmapStripping)
            {
                ShaderUtil.CalculateLightmapStrippingFromCurrentScene();
            }
            if (calcFogStripping)
            {
                ShaderUtil.CalculateFogStrippingFromCurrentScene();
            }
        }

        private void ShaderPreloadGUI()
        {
            EditorGUILayout.Space();
            GUILayout.Label(Styles.shaderPreloadSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_PreloadedShaders, true, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            GUILayout.Label(string.Format("Currently tracked: {0} shaders {1} total variants", ShaderUtil.GetCurrentShaderVariantCollectionShaderCount(), ShaderUtil.GetCurrentShaderVariantCollectionVariantCount()), new GUILayoutOption[0]);
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Styles.shaderPreloadSave, EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                string message = "Save shader variant collection";
                string str2 = EditorUtility.SaveFilePanelInProject("Save Shader Variant Collection", "NewShaderVariants", "shadervariants", message, ProjectWindowUtil.GetActiveFolderPath());
                if (!string.IsNullOrEmpty(str2))
                {
                    ShaderUtil.SaveCurrentShaderVariantCollection(str2);
                }
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(Styles.shaderPreloadClear, EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                ShaderUtil.ClearCurrentShaderVariantCollection();
            }
            EditorGUILayout.EndHorizontal();
        }

        internal class BuiltinShaderSettings
        {
            private readonly GUIContent m_Label;
            private readonly SerializedProperty m_Mode;
            private readonly SerializedProperty m_Shader;

            internal BuiltinShaderSettings(string label, string name, SerializedObject serializedObject)
            {
                this.m_Mode = serializedObject.FindProperty(name + ".m_Mode");
                this.m_Shader = serializedObject.FindProperty(name + ".m_Shader");
                this.m_Label = EditorGUIUtility.TextContent(label);
            }

            internal void DoGUI()
            {
                EditorGUILayout.PropertyField(this.m_Mode, this.m_Label, new GUILayoutOption[0]);
                if (this.m_Mode.intValue == 2)
                {
                    EditorGUILayout.PropertyField(this.m_Shader, new GUILayoutOption[0]);
                }
                EditorGUILayout.Space();
            }

            internal enum BuiltinShaderMode
            {
                None,
                Builtin,
                Custom
            }
        }

        internal class Styles
        {
            public static readonly GUIContent builtinSettings = EditorGUIUtility.TextContent("Built-in shader settings");
            public static readonly GUIContent fogExp = EditorGUIUtility.TextContent("Exponential|Include support for Exponential fog.");
            public static readonly GUIContent fogExp2 = EditorGUIUtility.TextContent("Exponential Squared|Include support for Exponential Squared fog.");
            public static readonly GUIContent fogFromScene = EditorGUIUtility.TextContent("From current scene|Calculate fog modes used by the current scene.");
            public static readonly GUIContent fogLinear = EditorGUIUtility.TextContent("Linear|Include support for Linear fog.");
            public static readonly GUIContent fogModes = EditorGUIUtility.TextContent("Fog modes");
            public static readonly GUIContent lightmapDirCombined = EditorGUIUtility.TextContent("Baked Directional|Include support for baked directional lightmaps.");
            public static readonly GUIContent lightmapDirSeparate = EditorGUIUtility.TextContent("Baked Directional Specular|Include support for baked directional specular lightmaps.");
            public static readonly GUIContent lightmapDynamicDirCombined = EditorGUIUtility.TextContent("Realtime Directional|Include support for realtime directional lightmaps.");
            public static readonly GUIContent lightmapDynamicDirSeparate = EditorGUIUtility.TextContent("Realtime Directional Specular|Include support for realtime directional specular lightmaps.");
            public static readonly GUIContent lightmapDynamicPlain = EditorGUIUtility.TextContent("Realtime Non-Directional|Include support for realtime non-directional lightmaps.");
            public static readonly GUIContent lightmapFromScene = EditorGUIUtility.TextContent("From current scene|Calculate lightmap modes used by the current scene.");
            public static readonly GUIContent lightmapModes = EditorGUIUtility.TextContent("Lightmap modes");
            public static readonly GUIContent lightmapPlain = EditorGUIUtility.TextContent("Baked Non-Directional|Include support for baked non-directional lightmaps.");
            public static readonly GUIContent shaderPreloadClear = EditorGUIUtility.TextContent("Clear|Clear currently tracked shader variant information.");
            public static readonly GUIContent shaderPreloadSave = EditorGUIUtility.TextContent("Save to asset...|Save currently tracked shaders into a Shader Variant Manifest asset.");
            public static readonly GUIContent shaderPreloadSettings = EditorGUIUtility.TextContent("Shader preloading");
            public static readonly GUIContent shaderStrippingSettings = EditorGUIUtility.TextContent("Shader stripping");
        }
    }
}


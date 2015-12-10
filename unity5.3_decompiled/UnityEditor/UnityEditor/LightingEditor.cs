namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(RenderSettings))]
    internal class LightingEditor : Editor
    {
        private static readonly GUIContent[] kFullAmbientModes = new GUIContent[] { EditorGUIUtility.TextContent("Skybox"), EditorGUIUtility.TextContent("Gradient"), EditorGUIUtility.TextContent("Color") };
        private static readonly int[] kFullAmbientModeValues;
        private const string kShowLightingEditorKey = "ShowLightingEditor";
        protected SerializedProperty m_AmbientEquatorColor;
        protected SerializedProperty m_AmbientGroundColor;
        protected SerializedProperty m_AmbientIntensity;
        protected SerializedProperty m_AmbientMode;
        protected SerializedProperty m_AmbientSkyColor;
        protected SerializedProperty m_CustomReflection;
        protected SerializedProperty m_DefaultReflectionMode;
        protected SerializedProperty m_DefaultReflectionResolution;
        protected SerializedProperty m_EnvironmentLightingMode;
        protected SerializedObject m_lightmapSettings;
        private EditorWindow m_ParentWindow;
        protected SerializedProperty m_ReflectionBounces;
        protected SerializedProperty m_ReflectionCompression;
        protected SerializedProperty m_ReflectionIntensity;
        private AnimBool m_ShowAmbientBakeMode = new AnimBool();
        private bool m_ShowEditor;
        protected SerializedProperty m_SkyboxMaterial;
        protected SerializedProperty m_Sun;

        static LightingEditor()
        {
            int[] numArray1 = new int[3];
            numArray1[1] = 1;
            numArray1[2] = 3;
            kFullAmbientModeValues = numArray1;
        }

        public virtual void OnDisable()
        {
            SessionState.SetBool("ShowLightingEditor", this.m_ShowEditor);
            this.m_ShowAmbientBakeMode.valueChanged.RemoveAllListeners();
            this.m_ParentWindow = null;
        }

        public virtual void OnEnable()
        {
            this.m_Sun = base.serializedObject.FindProperty("m_Sun");
            this.m_AmbientMode = base.serializedObject.FindProperty("m_AmbientMode");
            this.m_AmbientSkyColor = base.serializedObject.FindProperty("m_AmbientSkyColor");
            this.m_AmbientEquatorColor = base.serializedObject.FindProperty("m_AmbientEquatorColor");
            this.m_AmbientGroundColor = base.serializedObject.FindProperty("m_AmbientGroundColor");
            this.m_AmbientIntensity = base.serializedObject.FindProperty("m_AmbientIntensity");
            this.m_ReflectionIntensity = base.serializedObject.FindProperty("m_ReflectionIntensity");
            this.m_ReflectionBounces = base.serializedObject.FindProperty("m_ReflectionBounces");
            this.m_SkyboxMaterial = base.serializedObject.FindProperty("m_SkyboxMaterial");
            this.m_DefaultReflectionMode = base.serializedObject.FindProperty("m_DefaultReflectionMode");
            this.m_DefaultReflectionResolution = base.serializedObject.FindProperty("m_DefaultReflectionResolution");
            this.m_CustomReflection = base.serializedObject.FindProperty("m_CustomReflection");
            this.m_lightmapSettings = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
            this.m_EnvironmentLightingMode = this.m_lightmapSettings.FindProperty("m_GISettings.m_EnvironmentLightingMode");
            this.m_ReflectionCompression = this.m_lightmapSettings.FindProperty("m_LightmapEditorSettings.m_ReflectionCompression");
            this.m_ShowEditor = SessionState.GetBool("ShowLightingEditor", true);
            this.m_ShowAmbientBakeMode.target = ShowAmbientField();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.m_lightmapSettings.Update();
            EditorGUILayout.Space();
            this.m_ShowEditor = EditorGUILayout.FoldoutTitlebar(this.m_ShowEditor, Styles.environmentHeader);
            if (this.m_ShowEditor)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_SkyboxMaterial, Styles.skyboxLabel, new GUILayoutOption[0]);
                Material objectReferenceValue = this.m_SkyboxMaterial.objectReferenceValue as Material;
                if ((objectReferenceValue != null) && !EditorMaterialUtility.IsBackgroundMaterial(objectReferenceValue))
                {
                    EditorGUILayout.HelpBox(Styles.skyboxWarning.text, MessageType.Warning);
                }
                EditorGUILayout.PropertyField(this.m_Sun, Styles.sunLabel, new GUILayoutOption[0]);
                EditorGUILayout.Space();
                EditorGUILayout.IntPopup(this.m_AmbientMode, kFullAmbientModes, kFullAmbientModeValues, Styles.ambientModeLabel, new GUILayoutOption[0]);
                EditorGUI.indentLevel++;
                switch (this.m_AmbientMode.intValue)
                {
                    case 0:
                        if (objectReferenceValue == null)
                        {
                            EditorGUILayout.PropertyField(this.m_AmbientSkyColor, Styles.ambient, new GUILayoutOption[0]);
                        }
                        break;

                    case 1:
                        EditorGUILayout.PropertyField(this.m_AmbientSkyColor, Styles.ambientUp, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_AmbientEquatorColor, Styles.ambientMid, new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_AmbientGroundColor, Styles.ambientDown, new GUILayoutOption[0]);
                        break;

                    case 3:
                        EditorGUILayout.PropertyField(this.m_AmbientSkyColor, Styles.ambient, new GUILayoutOption[0]);
                        break;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Slider(this.m_AmbientIntensity, 0f, 8f, Styles.ambientIntensity, new GUILayoutOption[0]);
                this.m_ShowAmbientBakeMode.target = ShowAmbientField();
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowAmbientBakeMode.faded))
                {
                    bool flag = Lightmapping.realtimeLightmapsEnabled && Lightmapping.bakedLightmapsEnabled;
                    EditorGUI.BeginDisabledGroup(!flag);
                    if (flag)
                    {
                        EditorGUILayout.PropertyField(this.m_EnvironmentLightingMode, Styles.SkyLightBaked, new GUILayoutOption[0]);
                    }
                    else
                    {
                        int index = !Lightmapping.bakedLightmapsEnabled ? 0 : 1;
                        EditorGUILayout.LabelField(Styles.SkyLightBaked, GUIContent.Temp(this.m_EnvironmentLightingMode.enumNames[index]), EditorStyles.popup, new GUILayoutOption[0]);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(this.m_DefaultReflectionMode, Styles.reflectionModeLabel, new GUILayoutOption[0]);
                EditorGUI.indentLevel++;
                switch (((DefaultReflectionMode) this.m_DefaultReflectionMode.intValue))
                {
                    case DefaultReflectionMode.FromSkybox:
                    {
                        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
                        EditorGUILayout.IntPopup(this.m_DefaultReflectionResolution, Styles.defaultReflectionSizes, Styles.defaultReflectionSizesValues, Styles.defaultReflectionResolution, options);
                        break;
                    }
                    case DefaultReflectionMode.Custom:
                        EditorGUILayout.PropertyField(this.m_CustomReflection, Styles.customReflection, new GUILayoutOption[0]);
                        break;
                }
                EditorGUILayout.PropertyField(this.m_ReflectionCompression, Styles.ReflectionCompression, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
                EditorGUILayout.Slider(this.m_ReflectionIntensity, 0f, 1f, Styles.reflectionIntensity, new GUILayoutOption[0]);
                EditorGUILayout.IntSlider(this.m_ReflectionBounces, 1, 5, Styles.reflectionBounces, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
                base.serializedObject.ApplyModifiedProperties();
                this.m_lightmapSettings.ApplyModifiedProperties();
            }
        }

        private static bool ShowAmbientField()
        {
            return (Lightmapping.realtimeLightmapsEnabled || Lightmapping.bakedLightmapsEnabled);
        }

        public EditorWindow parentWindow
        {
            get
            {
                return this.m_ParentWindow;
            }
            set
            {
                if (this.m_ParentWindow != null)
                {
                    this.m_ShowAmbientBakeMode.valueChanged.RemoveListener(new UnityAction(this.m_ParentWindow.Repaint));
                }
                this.m_ParentWindow = value;
                this.m_ShowAmbientBakeMode.valueChanged.AddListener(new UnityAction(this.m_ParentWindow.Repaint));
            }
        }

        internal class Styles
        {
            [CompilerGenerated]
            private static Func<int, GUIContent> <>f__am$cache16;
            public static readonly GUIContent ambient = EditorGUIUtility.TextContent("Ambient Color|The color used for the ambient light shining into the scene.");
            public static readonly GUIContent ambientDown = EditorGUIUtility.TextContent("Ground Color|Ambient lighting coming from below.");
            public static readonly GUIContent ambientIntensity = EditorGUIUtility.TextContent("Ambient Intensity|How much the light from the Ambient Source affects the scene.");
            public static readonly GUIContent ambientMid = EditorGUIUtility.TextContent("Equator Color|Ambient lighting coming from the sides.");
            public static readonly GUIContent ambientModeLabel = EditorGUIUtility.TextContent("Ambient Source|The source of the ambient light that shines into the scene.");
            public static readonly GUIContent ambientUp = EditorGUIUtility.TextContent("Sky Color|Ambient lighting coming from above.");
            public static readonly GUIContent createLight = EditorGUIUtility.TextContent("Create Light");
            public static readonly GUIContent customReflection = EditorGUIUtility.TextContent("Cubemap|Custom reflection cubemap.");
            public static readonly GUIContent defaultReflectionResolution = EditorGUIUtility.TextContent("Resolution|Cubemap resolution for default reflection.");
            public static GUIContent[] defaultReflectionSizes;
            public static int[] defaultReflectionSizesValues = new int[] { 0x80, 0x100, 0x200, 0x400 };
            public static readonly GUIContent environmentHeader = EditorGUIUtility.TextContent("Environment Lighting|Settings for the scene's surroundings, that can cast light into the scene.");
            public static readonly GUIContent reflectionBounces = EditorGUIUtility.TextContent("Reflection Bounces|How many times reflection reflects another reflection, for ex., if you set 1 bounce, a reflection will not reflect another reflection, and will show black.");
            public static readonly GUIContent ReflectionCompression = EditorGUIUtility.TextContent("Compression|If Auto is selected Reflection Probes would be compressed unless doing so would result in ugly artefacts, e.g. PVRTC compression is \"warp around\" compression, so it is impossible to have seamless cubemap.");
            public static readonly GUIContent reflectionIntensity = EditorGUIUtility.TextContent("Reflection Intensity|How much the skybox / custom cubemap reflection affects the scene.");
            public static readonly GUIContent reflectionModeLabel = EditorGUIUtility.TextContent("Reflection Source|Default reflection cubemap - custom or generated from current skybox.");
            public static readonly GUIContent skyboxLabel = EditorGUIUtility.TextContent("Skybox|A skybox is rendered behind everything else in the scene in order to give the impression of scenery that is far away such as the sky or mountains. If 'Skybox' is set as the Ambient Source, light from this is cast into the scene.");
            public static readonly GUIContent skyboxTint = EditorGUIUtility.TextContent("Skybox Tint");
            public static readonly GUIContent skyboxWarning = EditorGUIUtility.TextContent("Shader of this material does not support skybox rendering.");
            public static readonly GUIContent SkyLightBaked = EditorGUIUtility.TextContent("Ambient GI|Which of the two Global Illumination modes (Precomputed Realtime or Baked) that should handle the ambient light. Only needed if both GI modes are enabled.");
            public static readonly GUIContent skyLightColor = EditorGUIUtility.TextContent("Sky Light Color");
            public static readonly GUIContent sunLabel = EditorGUIUtility.TextContent("Sun|The light used by the procedural skybox. If none, the brightest directional light is used.");

            static Styles()
            {
                if (<>f__am$cache16 == null)
                {
                    <>f__am$cache16 = new Func<int, GUIContent>(LightingEditor.Styles.<defaultReflectionSizes>m__202);
                }
                defaultReflectionSizes = defaultReflectionSizesValues.Select<int, GUIContent>(<>f__am$cache16).ToArray<GUIContent>();
            }

            [CompilerGenerated]
            private static GUIContent <defaultReflectionSizes>m__202(int n)
            {
                return new GUIContent(n.ToString());
            }
        }
    }
}


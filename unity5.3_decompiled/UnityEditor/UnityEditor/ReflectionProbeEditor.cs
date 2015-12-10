namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;
    using UnityEngine.SceneManagement;

    [CustomEditor(typeof(ReflectionProbe)), CanEditMultipleObjects]
    internal class ReflectionProbeEditor : Editor
    {
        internal static Color kGizmoHandleReflectionProbe = new Color(1f, 0.8980392f, 0.6666667f, 1f);
        internal static Color kGizmoReflectionProbe = new Color(1f, 0.8980392f, 0.5803922f, 0.5019608f);
        private SerializedProperty m_BackgroundColor;
        private SerializedProperty m_BlendDistance;
        private BoxEditor m_BoxEditor = new BoxEditor(true, s_BoxHash);
        private SerializedProperty m_BoxOffset;
        private SerializedProperty m_BoxProjection;
        private SerializedProperty m_BoxSize;
        private SerializedProperty m_ClearFlags;
        private TextureInspector m_CubemapEditor;
        private SerializedProperty m_CullingMask;
        private SerializedProperty m_CustomBakedTexture;
        private SerializedProperty m_HDR;
        private SerializedProperty m_Importance;
        private SerializedProperty m_IntensityMultiplier;
        private float m_MipLevelPreview;
        private SerializedProperty m_Mode;
        private SerializedProperty[] m_NearAndFarProperties;
        private Vector3 m_OldTransformPosition = Vector3.zero;
        private Material m_ReflectiveMaterial;
        private SerializedProperty m_RefreshMode;
        private SerializedProperty m_RenderDynamicObjects;
        private SerializedProperty m_Resolution;
        private SerializedProperty m_ShadowDistance;
        private readonly AnimBool m_ShowBoxOptions = new AnimBool();
        private readonly AnimBool m_ShowProbeModeCustomOptions = new AnimBool();
        private readonly AnimBool m_ShowProbeModeRealtimeOptions = new AnimBool();
        private SerializedProperty m_TimeSlicingMode;
        private SerializedProperty m_UseOcclusionCulling;
        private static int s_BoxHash = "ReflectionProbeEditorHash".GetHashCode();
        private static ReflectionProbeEditor s_LastInteractedEditor;
        private static Mesh s_PlaneMesh;
        private static Mesh s_SphereMesh;

        private void BakeCustomReflectionProbe(ReflectionProbe probe, bool usePreviousAssetPath)
        {
            string assetPath = string.Empty;
            if (usePreviousAssetPath)
            {
                assetPath = AssetDatabase.GetAssetPath(probe.customBakedTexture);
            }
            string extension = !probe.hdr ? "png" : "exr";
            if (string.IsNullOrEmpty(assetPath) || (Path.GetExtension(assetPath) != ("." + extension)))
            {
                ReflectionProbe probe2;
                string pathWithoutExtension = FileUtil.GetPathWithoutExtension(SceneManager.GetActiveScene().path);
                if (string.IsNullOrEmpty(pathWithoutExtension))
                {
                    pathWithoutExtension = "Assets";
                }
                else if (!Directory.Exists(pathWithoutExtension))
                {
                    Directory.CreateDirectory(pathWithoutExtension);
                }
                string fileNameWithoutExtension = probe.name + (!probe.hdr ? "-reflection" : "-reflectionHDR") + "." + extension;
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(Path.Combine(pathWithoutExtension, fileNameWithoutExtension)));
                assetPath = EditorUtility.SaveFilePanelInProject("Save reflection probe's cubemap.", fileNameWithoutExtension, extension, string.Empty, pathWithoutExtension);
                if (string.IsNullOrEmpty(assetPath))
                {
                    return;
                }
                if (this.IsCollidingWithOtherProbes(assetPath, probe, out probe2) && !EditorUtility.DisplayDialog("Cubemap is used by other reflection probe", string.Format("'{0}' path is used by the game object '{1}', do you really want to overwrite it?", assetPath, probe2.name), "Yes", "No"))
                {
                    return;
                }
            }
            EditorUtility.DisplayProgressBar("Reflection Probes", "Baking " + assetPath, 0.5f);
            if (!Lightmapping.BakeReflectionProbe(probe, assetPath))
            {
                Debug.LogError("Failed to bake reflection probe to " + assetPath);
            }
            EditorUtility.ClearProgressBar();
        }

        private void DoBakeButton()
        {
            if (this.reflectionProbeTarget.mode == ReflectionProbeMode.Realtime)
            {
                EditorGUILayout.HelpBox("Baking of this reflection probe should be initiated from the scripting API because the type is 'Realtime'", MessageType.Info);
                if (!QualitySettings.realtimeReflectionProbes)
                {
                    EditorGUILayout.HelpBox("Realtime reflection probes are disabled in Quality Settings", MessageType.Warning);
                }
            }
            else if ((this.reflectionProbeTarget.mode == ReflectionProbeMode.Baked) && (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand))
            {
                EditorGUILayout.HelpBox("Baking of this reflection probe is automatic because this probe's type is 'Baked' and the Lighting window is using 'Auto Baking'. The cubemap created is stored in the GI cache.", MessageType.Info);
            }
            else
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(EditorGUIUtility.labelWidth);
                switch (this.reflectionProbeMode)
                {
                    case ReflectionProbeMode.Baked:
                        EditorGUI.BeginDisabledGroup(!this.reflectionProbeTarget.enabled);
                        if (EditorGUI.ButtonWithDropdownList(Styles.bakeButtonText, Styles.bakeButtonsText, new GenericMenu.MenuFunction2(this.OnBakeButton), new GUILayoutOption[0]))
                        {
                            Lightmapping.BakeReflectionProbeSnapshot(this.reflectionProbeTarget);
                            GUIUtility.ExitGUI();
                        }
                        EditorGUI.EndDisabledGroup();
                        break;

                    case ReflectionProbeMode.Custom:
                        if (EditorGUI.ButtonWithDropdownList(Styles.bakeButtonText, Styles.bakeCustomButtonsText, new GenericMenu.MenuFunction2(this.OnBakeCustomButton), new GUILayoutOption[0]))
                        {
                            this.BakeCustomReflectionProbe(this.reflectionProbeTarget, true);
                            GUIUtility.ExitGUI();
                        }
                        break;
                }
                GUILayout.EndHorizontal();
            }
        }

        private void DoBoxEditing()
        {
            ReflectionProbe target = (ReflectionProbe) this.target;
            Vector3 position = target.transform.position;
            Vector3 size = target.size;
            Vector3 center = target.center + position;
            if (this.m_BoxEditor.OnSceneGUI(Matrix4x4.identity, kGizmoReflectionProbe, kGizmoHandleReflectionProbe, true, ref center, ref size))
            {
                Undo.RecordObject(target, "Modified Reflection Probe AABB");
                Vector3 vector4 = center - position;
                this.ValidateAABB(ref vector4, ref size);
                target.size = size;
                target.center = vector4;
                EditorUtility.SetDirty(this.target);
            }
        }

        private void DoOriginEditing()
        {
            ReflectionProbe target = (ReflectionProbe) this.target;
            Vector3 position = target.transform.position;
            Vector3 size = target.size;
            Vector3 center = target.center + position;
            EditorGUI.BeginChangeCheck();
            Vector3 point = Handles.PositionHandle(position, Quaternion.identity);
            bool flag = EditorGUI.EndChangeCheck();
            if (!flag)
            {
                point = position;
                Vector3 vector5 = this.m_OldTransformPosition - point;
                flag = vector5.magnitude > 1E-05f;
                if (flag)
                {
                    center = target.center + this.m_OldTransformPosition;
                }
            }
            if (flag)
            {
                Undo.RecordObject(target, "Modified Reflection Probe Origin");
                Bounds bounds = new Bounds(center, size);
                point = bounds.ClosestPoint(point);
                Vector3 vector6 = point;
                target.transform.position = vector6;
                this.m_OldTransformPosition = vector6;
                target.center = bounds.center - point;
                EditorUtility.SetDirty(this.target);
            }
        }

        private void DoToolbar()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUI.changed = false;
            EditMode.SceneViewEditMode editMode = EditMode.editMode;
            EditorGUI.BeginChangeCheck();
            EditMode.DoInspectorToolbar(Styles.sceneViewEditModes, Styles.toolContents, this.GetBounds(), this);
            if (EditorGUI.EndChangeCheck())
            {
                s_LastInteractedEditor = this;
            }
            if (editMode != EditMode.editMode)
            {
                if (EditMode.editMode == EditMode.SceneViewEditMode.ReflectionProbeOrigin)
                {
                    this.m_OldTransformPosition = ((ReflectionProbe) this.target).transform.position;
                }
                if (Toolbar.get != null)
                {
                    Toolbar.get.Repaint();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            string baseSceneEditingToolText = Styles.baseSceneEditingToolText;
            if (this.sceneViewEditing)
            {
                int index = ArrayUtility.IndexOf<EditMode.SceneViewEditMode>(Styles.sceneViewEditModes, EditMode.editMode);
                if (index >= 0)
                {
                    baseSceneEditingToolText = Styles.toolNames[index].text;
                }
            }
            GUILayout.Label(baseSceneEditingToolText, Styles.richTextMiniLabel, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private Bounds GetBounds()
        {
            if (this.target is ReflectionProbe)
            {
                ReflectionProbe target = (ReflectionProbe) this.target;
                return target.bounds;
            }
            return new Bounds();
        }

        private float GetProbeIntensity(ReflectionProbe p)
        {
            if ((p == null) || (p.texture == null))
            {
                return 1f;
            }
            float intensity = p.intensity;
            if (TextureUtil.GetTextureColorSpaceString(p.texture) == "Linear")
            {
                intensity = Mathf.LinearToGammaSpace(intensity);
            }
            return intensity;
        }

        public override bool HasPreviewGUI()
        {
            if (base.targets.Length > 1)
            {
                return false;
            }
            if (this.ValidPreviewSetup())
            {
                Editor cubemapEditor = this.m_CubemapEditor;
                Editor.CreateCachedEditor(((ReflectionProbe) this.target).texture, null, ref cubemapEditor);
                this.m_CubemapEditor = cubemapEditor as TextureInspector;
            }
            return true;
        }

        private bool IsCollidingWithOtherProbes(string targetPath, ReflectionProbe targetProbe, out ReflectionProbe collidingProbe)
        {
            ReflectionProbe[] probeArray = Object.FindObjectsOfType<ReflectionProbe>().ToArray<ReflectionProbe>();
            collidingProbe = null;
            foreach (ReflectionProbe probe in probeArray)
            {
                if (((probe != targetProbe) && (probe.customBakedTexture != null)) && (AssetDatabase.GetAssetPath(probe.customBakedTexture) == targetPath))
                {
                    collidingProbe = probe;
                    return true;
                }
            }
            return false;
        }

        private bool IsReflectionProbeEditMode(EditMode.SceneViewEditMode editMode)
        {
            return ((editMode == EditMode.SceneViewEditMode.ReflectionProbeBox) || (editMode == EditMode.SceneViewEditMode.ReflectionProbeOrigin));
        }

        private void OnBakeButton(object data)
        {
            if (((int) data) == 0)
            {
                Lightmapping.BakeAllReflectionProbesSnapshots();
            }
        }

        private void OnBakeCustomButton(object data)
        {
            int num = (int) data;
            ReflectionProbe target = this.target as ReflectionProbe;
            if (num == 0)
            {
                this.BakeCustomReflectionProbe(target, false);
            }
        }

        public void OnDisable()
        {
            this.m_BoxEditor.OnDisable();
            Object.DestroyImmediate(this.m_ReflectiveMaterial);
            Object.DestroyImmediate(this.m_CubemapEditor);
        }

        public void OnEnable()
        {
            this.m_Mode = base.serializedObject.FindProperty("m_Mode");
            this.m_RefreshMode = base.serializedObject.FindProperty("m_RefreshMode");
            this.m_TimeSlicingMode = base.serializedObject.FindProperty("m_TimeSlicingMode");
            this.m_Resolution = base.serializedObject.FindProperty("m_Resolution");
            this.m_NearAndFarProperties = new SerializedProperty[] { base.serializedObject.FindProperty("m_NearClip"), base.serializedObject.FindProperty("m_FarClip") };
            this.m_ShadowDistance = base.serializedObject.FindProperty("m_ShadowDistance");
            this.m_Importance = base.serializedObject.FindProperty("m_Importance");
            this.m_BoxSize = base.serializedObject.FindProperty("m_BoxSize");
            this.m_BoxOffset = base.serializedObject.FindProperty("m_BoxOffset");
            this.m_CullingMask = base.serializedObject.FindProperty("m_CullingMask");
            this.m_ClearFlags = base.serializedObject.FindProperty("m_ClearFlags");
            this.m_BackgroundColor = base.serializedObject.FindProperty("m_BackGroundColor");
            this.m_HDR = base.serializedObject.FindProperty("m_HDR");
            this.m_BoxProjection = base.serializedObject.FindProperty("m_BoxProjection");
            this.m_IntensityMultiplier = base.serializedObject.FindProperty("m_IntensityMultiplier");
            this.m_BlendDistance = base.serializedObject.FindProperty("m_BlendDistance");
            this.m_CustomBakedTexture = base.serializedObject.FindProperty("m_CustomBakedTexture");
            this.m_RenderDynamicObjects = base.serializedObject.FindProperty("m_RenderDynamicObjects");
            this.m_UseOcclusionCulling = base.serializedObject.FindProperty("m_UseOcclusionCulling");
            ReflectionProbe target = this.target as ReflectionProbe;
            this.m_ShowProbeModeRealtimeOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowProbeModeCustomOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowBoxOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowProbeModeRealtimeOptions.value = target.mode == ReflectionProbeMode.Realtime;
            this.m_ShowProbeModeCustomOptions.value = target.mode == ReflectionProbeMode.Custom;
            this.m_ShowBoxOptions.value = (!this.m_BoxSize.hasMultipleDifferentValues && !this.m_BoxOffset.hasMultipleDifferentValues) && (target.type == ReflectionProbeType.Cube);
            this.m_BoxEditor.OnEnable();
            this.m_BoxEditor.SetAlwaysDisplayHandles(true);
            this.m_BoxEditor.allowNegativeSize = false;
            this.m_OldTransformPosition = ((ReflectionProbe) this.target).transform.position;
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            if (base.targets.Length == 1)
            {
                this.DoToolbar();
            }
            this.m_ShowProbeModeRealtimeOptions.target = this.reflectionProbeMode == ReflectionProbeMode.Realtime;
            this.m_ShowProbeModeCustomOptions.target = this.reflectionProbeMode == ReflectionProbeMode.Custom;
            EditorGUILayout.IntPopup(this.m_Mode, Styles.reflectionProbeMode, Styles.reflectionProbeModeValues, Styles.typeText, new GUILayoutOption[0]);
            if (!this.m_Mode.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowProbeModeCustomOptions.faded))
                {
                    EditorGUILayout.PropertyField(this.m_RenderDynamicObjects, Styles.renderDynamicObjects, new GUILayoutOption[0]);
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = this.m_CustomBakedTexture.hasMultipleDifferentValues;
                    Object obj2 = EditorGUILayout.ObjectField(Styles.customCubemapText, this.m_CustomBakedTexture.objectReferenceValue, typeof(Cubemap), false, new GUILayoutOption[0]);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_CustomBakedTexture.objectReferenceValue = obj2;
                    }
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowProbeModeRealtimeOptions.faded))
                {
                    EditorGUILayout.PropertyField(this.m_RefreshMode, Styles.refreshMode, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_TimeSlicingMode, Styles.timeSlicing, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            GUILayout.Label(Styles.runtimeSettingsHeader, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.m_Importance, Styles.importanceText, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_IntensityMultiplier, Styles.intensityText, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_BoxProjection, Styles.boxProjectionText, new GUILayoutOption[0]);
            bool flag2 = SceneView.IsUsingDeferredRenderingPath() && (GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredReflections) != BuiltinShaderMode.Disabled);
            EditorGUI.BeginDisabledGroup(!flag2);
            EditorGUILayout.PropertyField(this.m_BlendDistance, Styles.blendDistanceText, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowBoxOptions.faded))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_BoxSize, Styles.sizeText, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_BoxOffset, Styles.centerText, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Vector3 center = this.m_BoxOffset.vector3Value;
                    Vector3 size = this.m_BoxSize.vector3Value;
                    if (this.ValidateAABB(ref center, ref size))
                    {
                        this.m_BoxOffset.vector3Value = center;
                        this.m_BoxSize.vector3Value = size;
                    }
                }
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            GUILayout.Label(Styles.captureCubemapHeaderText, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
            EditorGUILayout.IntPopup(this.m_Resolution, Styles.renderTextureSizes, Styles.renderTextureSizesValues, Styles.resolutionText, options);
            EditorGUILayout.PropertyField(this.m_HDR, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ShadowDistance, new GUILayoutOption[0]);
            EditorGUILayout.IntPopup(this.m_ClearFlags, Styles.clearFlags, Styles.clearFlagsValues, Styles.clearFlagsText, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_BackgroundColor, Styles.backgroundColorText, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_CullingMask, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_UseOcclusionCulling, new GUILayoutOption[0]);
            EditorGUILayout.PropertiesField(EditorGUI.s_ClipingPlanesLabel, this.m_NearAndFarProperties, EditorGUI.s_NearAndFarLabels, 35f, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            if (base.targets.Length == 1)
            {
                ReflectionProbe target = (ReflectionProbe) this.target;
                if ((target.mode == ReflectionProbeMode.Custom) && (target.customBakedTexture != null))
                {
                    Cubemap customBakedTexture = target.customBakedTexture as Cubemap;
                    if ((customBakedTexture != null) && (customBakedTexture.mipmapCount == 1))
                    {
                        EditorGUILayout.HelpBox("No mipmaps in the cubemap, Smoothness value in Standard shader will be ignored.", MessageType.Warning);
                    }
                }
            }
            this.DoBakeButton();
            EditorGUILayout.Space();
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnPreSceneGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                ReflectionProbe target = (ReflectionProbe) this.target;
                if (this.reflectiveMaterial != null)
                {
                    Matrix4x4 matrix = new Matrix4x4();
                    Material reflectiveMaterial = this.reflectiveMaterial;
                    if (target.type == ReflectionProbeType.Cube)
                    {
                        float mipLevelForRendering = 0f;
                        TextureInspector cubemapEditor = this.m_CubemapEditor;
                        if (cubemapEditor != null)
                        {
                            mipLevelForRendering = cubemapEditor.GetMipLevelForRendering();
                        }
                        reflectiveMaterial.mainTexture = target.texture;
                        reflectiveMaterial.SetMatrix("_CubemapRotation", Matrix4x4.identity);
                        reflectiveMaterial.SetFloat("_Mip", mipLevelForRendering);
                        reflectiveMaterial.SetFloat("_Alpha", 0f);
                        reflectiveMaterial.SetFloat("_Intensity", this.GetProbeIntensity(target));
                        float x = target.transform.lossyScale.magnitude * 0.5f;
                        matrix.SetTRS(target.transform.position, Quaternion.identity, new Vector3(x, x, x));
                        Graphics.DrawMesh(sphereMesh, matrix, this.reflectiveMaterial, 0, SceneView.currentDrawingSceneView.camera);
                    }
                    else
                    {
                        reflectiveMaterial.SetTexture("_MainTex", target.texture);
                        reflectiveMaterial.SetFloat("_ReflectionProbeType", 1f);
                        reflectiveMaterial.SetFloat("_Intensity", 1f);
                        Vector3 s = new Vector3();
                        s = (Vector3) (target.transform.lossyScale * 0.2f);
                        s.x *= -1f;
                        s.z *= -1f;
                        matrix.SetTRS(target.transform.position, target.transform.rotation * Quaternion.AngleAxis(90f, Vector3.right), s);
                        Graphics.DrawMesh(planeMesh, matrix, this.reflectiveMaterial, 0, SceneView.currentDrawingSceneView.camera);
                    }
                }
            }
        }

        public override void OnPreviewGUI(Rect position, GUIStyle style)
        {
            if (!this.ValidPreviewSetup())
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                Color color = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                GUILayout.Label("Reflection Probe not baked yet", new GUILayoutOption[0]);
                GUI.color = color;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                ReflectionProbe target = this.target as ReflectionProbe;
                if (((target != null) && (target.texture != null)) && (base.targets.Length == 1))
                {
                    Editor cubemapEditor = this.m_CubemapEditor;
                    Editor.CreateCachedEditor(target.texture, null, ref cubemapEditor);
                    this.m_CubemapEditor = cubemapEditor as TextureInspector;
                }
                if (this.m_CubemapEditor != null)
                {
                    this.m_CubemapEditor.SetCubemapIntensity(this.GetProbeIntensity((ReflectionProbe) this.target));
                    this.m_CubemapEditor.OnPreviewGUI(position, style);
                }
            }
        }

        public override void OnPreviewSettings()
        {
            if (this.ValidPreviewSetup())
            {
                this.m_CubemapEditor.mipLevel = this.m_MipLevelPreview;
                EditorGUI.BeginChangeCheck();
                this.m_CubemapEditor.OnPreviewSettings();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorApplication.SetSceneRepaintDirty();
                    this.m_MipLevelPreview = this.m_CubemapEditor.mipLevel;
                }
            }
        }

        public void OnSceneGUI()
        {
            if (this.sceneViewEditing)
            {
                switch (EditMode.editMode)
                {
                    case EditMode.SceneViewEditMode.ReflectionProbeBox:
                        this.DoBoxEditing();
                        break;

                    case EditMode.SceneViewEditMode.ReflectionProbeOrigin:
                        this.DoOriginEditing();
                        break;
                }
            }
        }

        [DrawGizmo(GizmoType.Active)]
        private static void RenderBoxGizmo(ReflectionProbe reflectionProbe, GizmoType gizmoType)
        {
            if ((s_LastInteractedEditor != null) && (s_LastInteractedEditor.sceneViewEditing && (EditMode.editMode == EditMode.SceneViewEditMode.ReflectionProbeBox)))
            {
                Color color = Gizmos.color;
                Gizmos.color = kGizmoReflectionProbe;
                Gizmos.DrawCube(reflectionProbe.transform.position + reflectionProbe.center, (Vector3) (-1f * reflectionProbe.size));
                Gizmos.color = color;
            }
        }

        private bool ValidateAABB(ref Vector3 center, ref Vector3 size)
        {
            ReflectionProbe target = (ReflectionProbe) this.target;
            Vector3 position = target.transform.position;
            Bounds bounds = new Bounds(center + position, size);
            if (bounds.Contains(position))
            {
                return false;
            }
            bounds.Encapsulate(position);
            center = bounds.center - position;
            size = bounds.size;
            return true;
        }

        private bool ValidPreviewSetup()
        {
            ReflectionProbe target = (ReflectionProbe) this.target;
            return ((target != null) && (target.texture != null));
        }

        private static Mesh planeMesh
        {
            get
            {
                if (s_PlaneMesh == null)
                {
                }
                return (s_PlaneMesh = Resources.GetBuiltinResource(typeof(Mesh), "New-Plane.fbx") as Mesh);
            }
        }

        private ReflectionProbeMode reflectionProbeMode
        {
            get
            {
                return this.reflectionProbeTarget.mode;
            }
        }

        private ReflectionProbe reflectionProbeTarget
        {
            get
            {
                return (ReflectionProbe) this.target;
            }
        }

        private Material reflectiveMaterial
        {
            get
            {
                if (this.m_ReflectiveMaterial == null)
                {
                    this.m_ReflectiveMaterial = (Material) Object.Instantiate(EditorGUIUtility.Load("Previews/PreviewCubemapMaterial.mat"));
                    this.m_ReflectiveMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return this.m_ReflectiveMaterial;
            }
        }

        private bool sceneViewEditing
        {
            get
            {
                return (this.IsReflectionProbeEditMode(EditMode.editMode) && EditMode.IsOwner(this));
            }
        }

        private static Mesh sphereMesh
        {
            get
            {
                if (s_SphereMesh == null)
                {
                }
                return (s_SphereMesh = Resources.GetBuiltinResource(typeof(Mesh), "New-Sphere.fbx") as Mesh);
            }
        }

        private static class Styles
        {
            [CompilerGenerated]
            private static Func<int, GUIContent> <>f__am$cache22;
            public static GUIContent backgroundColorText = new GUIContent("Background", "Camera clears the screen to this color before rendering.");
            public static string[] bakeButtonsText = new string[] { "Bake All Reflection Probes" };
            public static string bakeButtonText = "Bake";
            public static string[] bakeCustomButtonsText = new string[] { "Bake as new Cubemap..." };
            public static string baseSceneEditingToolText;
            public static GUIContent blendDistanceText = new GUIContent("Blend Distance", "Area around the probe where it is blended with other probes. Only used in deferred probes.");
            public static GUIContent boxProjectionText = new GUIContent("Box Projection", "Box projection is useful for reflections in enclosed spaces where some parrallax and movement in the reflection is wanted. If not set then cubemap reflection will we treated as coming infinite far away. And within this zone objects with the Standard shader will receive this probe's cubemap.");
            public static GUIContent captureCubemapHeaderText = new GUIContent("Cubemap capture settings");
            public static GUIContent centerText = new GUIContent("Probe Origin");
            public static GUIContent[] clearFlags;
            public static GUIContent clearFlagsText = new GUIContent("Clear Flags");
            public static int[] clearFlagsValues;
            public static GUIStyle commandStyle;
            public static GUIContent customCubemapText = new GUIContent("Cubemap");
            public static string editBoundsText = "Edit Bounds";
            public static GUIContent editorUpdateText = new GUIContent("Editor Update");
            public static GUIContent importanceText = new GUIContent("Importance");
            public static GUIContent intensityText = new GUIContent("Intensity");
            public static GUIContent[] reflectionProbeMode = new GUIContent[] { new GUIContent("Baked"), new GUIContent("Custom"), new GUIContent("Realtime") };
            public static int[] reflectionProbeModeValues;
            public static GUIContent refreshMode = new GUIContent("Refresh Mode", "Controls how this probe refreshes in the Player");
            public static GUIContent renderDynamicObjects = new GUIContent("Dynamic Objects", "If enabled dynamic objects are also rendered into the cubemap");
            public static GUIContent[] renderTextureSizes;
            public static int[] renderTextureSizesValues;
            public static GUIContent resolutionText = new GUIContent("Resolution");
            public static GUIStyle richTextMiniLabel = new GUIStyle(EditorStyles.miniLabel);
            public static GUIContent runtimeSettingsHeader = new GUIContent("Runtime settings", "These settings are used by objects when they render with the cubemap of this probe");
            public static EditMode.SceneViewEditMode[] sceneViewEditModes;
            public static GUIContent sizeText = new GUIContent("Size");
            public static GUIContent skipFramesText = new GUIContent("Skip frames");
            public static GUIContent timeSlicing = new GUIContent("Time Slicing", "If enabled this probe will update over several frames, to help reduce the impact on the frame rate");
            public static GUIContent[] toolContents;
            public static GUIContent[] toolNames;
            public static GUIContent typeText = new GUIContent("Type", "'Baked Cubemap' uses the 'Auto Baking' mode from the Lighting window. If it is enabled then baking is automatic otherwise manual bake is needed (use the bake button below). \n'Custom' can be used if a custom cubemap is wanted. \n'Realtime' can be used to dynamically re-render the cubemap during runtime (via scripting).");

            static Styles()
            {
                int[] numArray1 = new int[3];
                numArray1[1] = 2;
                numArray1[2] = 1;
                reflectionProbeModeValues = numArray1;
                renderTextureSizesValues = new int[] { 0x10, 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800 };
                if (<>f__am$cache22 == null)
                {
                    <>f__am$cache22 = new Func<int, GUIContent>(ReflectionProbeEditor.Styles.<renderTextureSizes>m__1A6);
                }
                renderTextureSizes = renderTextureSizesValues.Select<int, GUIContent>(<>f__am$cache22).ToArray<GUIContent>();
                clearFlags = new GUIContent[] { new GUIContent("Skybox"), new GUIContent("Solid Color") };
                clearFlagsValues = new int[] { 1, 2 };
                toolContents = new GUIContent[] { EditorGUIUtility.IconContent("EditCollider"), EditorGUIUtility.IconContent("MoveTool", "|Move the selected objects.") };
                sceneViewEditModes = new EditMode.SceneViewEditMode[] { EditMode.SceneViewEditMode.ReflectionProbeBox, EditMode.SceneViewEditMode.ReflectionProbeOrigin };
                baseSceneEditingToolText = "<color=grey>Probe Scene Editing Mode:</color> ";
                toolNames = new GUIContent[] { new GUIContent(baseSceneEditingToolText + "Box Projection Bounds", string.Empty), new GUIContent(baseSceneEditingToolText + "Probe Origin", string.Empty) };
                commandStyle = "Command";
                richTextMiniLabel.richText = true;
            }

            [CompilerGenerated]
            private static GUIContent <renderTextureSizes>m__1A6(int n)
            {
                return new GUIContent(n.ToString());
            }
        }
    }
}


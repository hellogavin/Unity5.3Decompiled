namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEditor.VersionControl;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(SpeedTreeImporter)), CanEditMultipleObjects]
    internal class SpeedTreeImporterInspector : AssetImporterInspector
    {
        [CompilerGenerated]
        private static Func<SpeedTreeImporter, bool> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<SpeedTreeImporter, string> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache12;
        [CompilerGenerated]
        private static Func<SpeedTreeImporter, int> <>f__am$cache13;
        [CompilerGenerated]
        private static Func<string, GUIContent> <>f__am$cache14;
        [CompilerGenerated]
        private static Func<float, string> <>f__am$cache15;
        [CompilerGenerated]
        private static Func<LODGroupGUI.LODInfo, bool> <>f__am$cache16;
        [CompilerGenerated]
        private static Func<LODGroupGUI.LODInfo, int> <>f__am$cache17;
        [CompilerGenerated]
        private static Func<LODGroupGUI.LODInfo, bool> <>f__am$cache18;
        [CompilerGenerated]
        private static Func<LODGroupGUI.LODInfo, int> <>f__am$cache19;
        private const float kFeetToMetersRatio = 0.3048f;
        private SerializedProperty m_AlphaTestRef;
        private SerializedProperty m_AnimateCrossFading;
        private SerializedProperty m_BillboardTransitionCrossFadeWidth;
        private SerializedProperty m_EnableSmoothLOD;
        private SerializedProperty m_FadeOutWidth;
        private SerializedProperty m_HueVariation;
        private SerializedProperty m_LODSettings;
        private readonly int m_LODSliderId = "LODSliderIDHash".GetHashCode();
        private SerializedProperty m_MainColor;
        private SerializedProperty m_ScaleFactor;
        private int m_SelectedLODRange;
        private int m_SelectedLODSlider = -1;
        private SerializedProperty m_Shininess;
        private readonly AnimBool m_ShowCrossFadeWidthOptions = new AnimBool();
        private readonly AnimBool m_ShowSmoothLODOptions = new AnimBool();
        private SerializedProperty m_SpecColor;

        protected override bool ApplyRevertGUIButtons()
        {
            EditorGUI.BeginDisabledGroup(!this.HasModified());
            base.RevertButton();
            bool flag = base.ApplyButton("Apply Prefab");
            EditorGUI.EndDisabledGroup();
            bool upgradeMaterials = this.upgradeMaterials;
            GUIContent content = (!this.HasModified() && !upgradeMaterials) ? Styles.Regenerate : Styles.ApplyAndGenerate;
            if (!GUILayout.Button(content, new GUILayoutOption[0]))
            {
                return flag;
            }
            bool flag3 = this.HasModified();
            if (flag3)
            {
                this.Apply();
            }
            if (upgradeMaterials)
            {
                foreach (SpeedTreeImporter importer in this.importers)
                {
                    importer.SetMaterialVersionToCurrent();
                }
            }
            this.GenerateMaterials();
            if (!flag3 && !upgradeMaterials)
            {
                return flag;
            }
            base.ApplyAndImport();
            return true;
        }

        public bool CanUnifyLODConfig()
        {
            return (!base.serializedObject.FindProperty("m_HasBillboard").hasMultipleDifferentValues && !this.m_LODSettings.FindPropertyRelative("Array.size").hasMultipleDifferentValues);
        }

        private void DrawLODLevelSlider(Rect sliderPosition, List<LODGroupGUI.LODInfo> lods)
        {
            int controlID = GUIUtility.GetControlID(this.m_LODSliderId, FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                {
                    Rect rect = sliderPosition;
                    rect.x -= 5f;
                    rect.width += 10f;
                    if (rect.Contains(current.mousePosition))
                    {
                        current.Use();
                        GUIUtility.hotControl = controlID;
                        if (<>f__am$cache16 == null)
                        {
                            <>f__am$cache16 = lod => lod.ScreenPercent > 0.5f;
                        }
                        if (<>f__am$cache17 == null)
                        {
                            <>f__am$cache17 = x => x.LODLevel;
                        }
                        IOrderedEnumerable<LODGroupGUI.LODInfo> collection = lods.Where<LODGroupGUI.LODInfo>(<>f__am$cache16).OrderByDescending<LODGroupGUI.LODInfo, int>(<>f__am$cache17);
                        if (<>f__am$cache18 == null)
                        {
                            <>f__am$cache18 = lod => lod.ScreenPercent <= 0.5f;
                        }
                        if (<>f__am$cache19 == null)
                        {
                            <>f__am$cache19 = x => x.LODLevel;
                        }
                        IOrderedEnumerable<LODGroupGUI.LODInfo> enumerable2 = lods.Where<LODGroupGUI.LODInfo>(<>f__am$cache18).OrderBy<LODGroupGUI.LODInfo, int>(<>f__am$cache19);
                        List<LODGroupGUI.LODInfo> list = new List<LODGroupGUI.LODInfo>();
                        list.AddRange(collection);
                        list.AddRange(enumerable2);
                        foreach (LODGroupGUI.LODInfo info in list)
                        {
                            if (info.m_ButtonPosition.Contains(current.mousePosition))
                            {
                                this.m_SelectedLODSlider = info.LODLevel;
                                this.m_SelectedLODRange = info.LODLevel;
                                break;
                            }
                            if (info.m_RangePosition.Contains(current.mousePosition))
                            {
                                this.m_SelectedLODSlider = -1;
                                this.m_SelectedLODRange = info.LODLevel;
                                break;
                            }
                        }
                    }
                    break;
                }
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (((GUIUtility.hotControl == controlID) && (this.m_SelectedLODSlider >= 0)) && (lods[this.m_SelectedLODSlider] != null))
                    {
                        current.Use();
                        LODGroupGUI.SetSelectedLODLevelPercentage(Mathf.Clamp01(1f - ((current.mousePosition.x - sliderPosition.x) / sliderPosition.width)) - 0.001f, this.m_SelectedLODSlider, lods);
                        this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODSlider).FindPropertyRelative("height").floatValue = lods[this.m_SelectedLODSlider].RawScreenPercent;
                    }
                    break;

                case EventType.Repaint:
                    LODGroupGUI.DrawLODSlider(sliderPosition, lods, this.m_SelectedLODRange);
                    break;
            }
        }

        private void GenerateMaterials()
        {
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = im => im.materialFolderPath;
            }
            string[] searchInFolders = this.importers.Select<SpeedTreeImporter, string>(<>f__am$cache11).ToArray<string>();
            if (<>f__am$cache12 == null)
            {
                <>f__am$cache12 = guid => AssetDatabase.GUIDToAssetPath(guid);
            }
            string[] assets = AssetDatabase.FindAssets("t:Material", searchInFolders).Select<string, string>(<>f__am$cache12).ToArray<string>();
            bool flag = true;
            if (assets.Length > 0)
            {
                flag = Provider.PromptAndCheckoutIfNeeded(assets, string.Format("Materials will be checked out in:\n{0}", string.Join("\n", searchInFolders)));
            }
            if (flag)
            {
                foreach (SpeedTreeImporter importer in this.importers)
                {
                    importer.GenerateMaterials();
                }
            }
        }

        internal List<LODGroupGUI.LODInfo> GetLODInfoArray(Rect area)
        {
            <GetLODInfoArray>c__AnonStorey7A storeya = new <GetLODInfoArray>c__AnonStorey7A {
                <>f__this = this,
                lodCount = this.m_LODSettings.arraySize
            };
            return LODGroupGUI.CreateLODInfos(storeya.lodCount, area, new Func<int, string>(storeya.<>m__124), new Func<int, float>(storeya.<>m__125));
        }

        public bool HasSameLODConfig()
        {
            if (base.serializedObject.FindProperty("m_HasBillboard").hasMultipleDifferentValues)
            {
                return false;
            }
            if (this.m_LODSettings.FindPropertyRelative("Array.size").hasMultipleDifferentValues)
            {
                return false;
            }
            for (int i = 0; i < this.m_LODSettings.arraySize; i++)
            {
                if (this.m_LODSettings.GetArrayElementAtIndex(i).FindPropertyRelative("height").hasMultipleDifferentValues)
                {
                    return false;
                }
            }
            return true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            this.m_ShowSmoothLODOptions.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowCrossFadeWidthOptions.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        private void OnEnable()
        {
            this.m_LODSettings = base.serializedObject.FindProperty("m_LODSettings");
            this.m_EnableSmoothLOD = base.serializedObject.FindProperty("m_EnableSmoothLODTransition");
            this.m_AnimateCrossFading = base.serializedObject.FindProperty("m_AnimateCrossFading");
            this.m_BillboardTransitionCrossFadeWidth = base.serializedObject.FindProperty("m_BillboardTransitionCrossFadeWidth");
            this.m_FadeOutWidth = base.serializedObject.FindProperty("m_FadeOutWidth");
            this.m_MainColor = base.serializedObject.FindProperty("m_MainColor");
            this.m_SpecColor = base.serializedObject.FindProperty("m_SpecColor");
            this.m_HueVariation = base.serializedObject.FindProperty("m_HueVariation");
            this.m_Shininess = base.serializedObject.FindProperty("m_Shininess");
            this.m_AlphaTestRef = base.serializedObject.FindProperty("m_AlphaTestRef");
            this.m_ScaleFactor = base.serializedObject.FindProperty("m_ScaleFactor");
            this.m_ShowSmoothLODOptions.value = this.m_EnableSmoothLOD.hasMultipleDifferentValues || this.m_EnableSmoothLOD.boolValue;
            this.m_ShowSmoothLODOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowCrossFadeWidthOptions.value = this.m_AnimateCrossFading.hasMultipleDifferentValues || !this.m_AnimateCrossFading.boolValue;
            this.m_ShowCrossFadeWidthOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            this.ShowMeshGUI();
            this.ShowMaterialGUI();
            this.ShowLODGUI();
            EditorGUILayout.Space();
            if (this.upgradeMaterials)
            {
                EditorGUILayout.HelpBox(string.Format("SpeedTree materials need to be upgraded. Please back them up (if modified manually) then hit the \"{0}\" button below.", Styles.ApplyAndGenerate.text), MessageType.Warning);
            }
            base.ApplyRevertGUI();
        }

        private void OnResetLODMenuClick(object userData)
        {
            float[] lODHeights = (userData as SpeedTreeImporter).LODHeights;
            for (int i = 0; i < lODHeights.Length; i++)
            {
                this.m_LODSettings.GetArrayElementAtIndex(i).FindPropertyRelative("height").floatValue = lODHeights[i];
            }
        }

        private void ShowLODGUI()
        {
            this.m_ShowSmoothLODOptions.target = this.m_EnableSmoothLOD.hasMultipleDifferentValues || this.m_EnableSmoothLOD.boolValue;
            this.m_ShowCrossFadeWidthOptions.target = this.m_AnimateCrossFading.hasMultipleDifferentValues || !this.m_AnimateCrossFading.boolValue;
            GUILayout.Label(Styles.LODHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_EnableSmoothLOD, Styles.SmoothLOD, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowSmoothLODOptions.faded))
            {
                EditorGUILayout.PropertyField(this.m_AnimateCrossFading, Styles.AnimateCrossFading, new GUILayoutOption[0]);
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowCrossFadeWidthOptions.faded))
                {
                    EditorGUILayout.Slider(this.m_BillboardTransitionCrossFadeWidth, 0f, 1f, Styles.CrossFadeWidth, new GUILayoutOption[0]);
                    EditorGUILayout.Slider(this.m_FadeOutWidth, 0f, 1f, Styles.FadeOutWidth, new GUILayoutOption[0]);
                }
                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            if (this.HasSameLODConfig())
            {
                EditorGUILayout.Space();
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect area = GUILayoutUtility.GetRect((float) 0f, (float) 30f, options);
                List<LODGroupGUI.LODInfo> lODInfoArray = this.GetLODInfoArray(area);
                this.DrawLODLevelSlider(area, lODInfoArray);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                if ((this.m_SelectedLODRange != -1) && (lODInfoArray.Count > 0))
                {
                    EditorGUILayout.LabelField(lODInfoArray[this.m_SelectedLODRange].LODName + " Options", EditorStyles.boldLabel, new GUILayoutOption[0]);
                    bool flag = (this.m_SelectedLODRange == (lODInfoArray.Count - 1)) && this.importers[0].hasBillboard;
                    EditorGUILayout.PropertyField(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("castShadows"), Styles.CastShadows, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("receiveShadows"), Styles.ReceiveShadows, new GUILayoutOption[0]);
                    SerializedProperty property = this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("useLightProbes");
                    EditorGUILayout.PropertyField(property, Styles.UseLightProbes, new GUILayoutOption[0]);
                    if ((!property.hasMultipleDifferentValues && property.boolValue) && flag)
                    {
                        EditorGUILayout.HelpBox("Enabling Light Probe for billboards breaks batched rendering and may cause performance problem.", MessageType.Warning);
                    }
                    EditorGUILayout.PropertyField(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("enableBump"), Styles.EnableBump, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("enableHue"), Styles.EnableHue, new GUILayoutOption[0]);
                    if (<>f__am$cache13 == null)
                    {
                        <>f__am$cache13 = im => im.bestWindQuality;
                    }
                    int num = this.importers.Min<SpeedTreeImporter>(<>f__am$cache13);
                    if (num > 0)
                    {
                        if (flag)
                        {
                            num = (num < 1) ? 0 : 1;
                        }
                        if (<>f__am$cache14 == null)
                        {
                            <>f__am$cache14 = s => new GUIContent(s);
                        }
                        EditorGUILayout.Popup(this.m_LODSettings.GetArrayElementAtIndex(this.m_SelectedLODRange).FindPropertyRelative("windQuality"), SpeedTreeImporter.windQualityNames.Take<string>((num + 1)).Select<string, GUIContent>(<>f__am$cache14).ToArray<GUIContent>(), Styles.WindQuality, new GUILayoutOption[0]);
                    }
                }
            }
            else
            {
                if (this.CanUnifyLODConfig())
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    Rect rect = GUILayoutUtility.GetRect(Styles.ResetLOD, EditorStyles.miniButton);
                    if (GUI.Button(rect, Styles.ResetLOD, EditorStyles.miniButton))
                    {
                        GenericMenu menu = new GenericMenu();
                        IEnumerator<SpeedTreeImporter> enumerator = base.targets.Cast<SpeedTreeImporter>().GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                SpeedTreeImporter current = enumerator.Current;
                                if (<>f__am$cache15 == null)
                                {
                                    <>f__am$cache15 = height => string.Format("{0:0}%", height * 100f);
                                }
                                string text = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(current.assetPath), string.Join(" | ", current.LODHeights.Select<float, string>(<>f__am$cache15).ToArray<string>()));
                                menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.OnResetLODMenuClick), current);
                            }
                        }
                        finally
                        {
                            if (enumerator == null)
                            {
                            }
                            enumerator.Dispose();
                        }
                        menu.DropDown(rect);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect rect3 = GUILayoutUtility.GetRect((float) 0f, (float) 30f, optionArray2);
                if (Event.current.type == EventType.Repaint)
                {
                    LODGroupGUI.DrawMixedValueLODSlider(rect3);
                }
            }
            EditorGUILayout.Space();
        }

        private void ShowMaterialGUI()
        {
            GUILayout.Label(Styles.MaterialsHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_MainColor, Styles.MainColor, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SpecColor, Styles.SpecColor, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_HueVariation, Styles.HueVariation, new GUILayoutOption[0]);
            EditorGUILayout.Slider(this.m_Shininess, 0.01f, 1f, Styles.Shininess, new GUILayoutOption[0]);
            EditorGUILayout.Slider(this.m_AlphaTestRef, 0f, 1f, Styles.AlphaTestRef, new GUILayoutOption[0]);
        }

        private void ShowMeshGUI()
        {
            GUILayout.Label(Styles.MeshesHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ScaleFactor, Styles.ScaleFactor, new GUILayoutOption[0]);
            if (!this.m_ScaleFactor.hasMultipleDifferentValues && Mathf.Approximately(this.m_ScaleFactor.floatValue, 0.3048f))
            {
                EditorGUILayout.HelpBox(Styles.ScaleFactorHelp.text, MessageType.Info);
            }
        }

        private SpeedTreeImporter[] importers
        {
            get
            {
                return base.targets.Cast<SpeedTreeImporter>().ToArray<SpeedTreeImporter>();
            }
        }

        private bool upgradeMaterials
        {
            get
            {
                if (<>f__am$cache10 == null)
                {
                    <>f__am$cache10 = i => i.materialsShouldBeRegenerated;
                }
                return this.importers.Any<SpeedTreeImporter>(<>f__am$cache10);
            }
        }

        [CompilerGenerated]
        private sealed class <GetLODInfoArray>c__AnonStorey7A
        {
            internal SpeedTreeImporterInspector <>f__this;
            internal int lodCount;

            internal string <>m__124(int i)
            {
                return (((i != (this.lodCount - 1)) || !(this.<>f__this.target as SpeedTreeImporter).hasBillboard) ? string.Format("LOD {0}", i) : "Billboard");
            }

            internal float <>m__125(int i)
            {
                return this.<>f__this.m_LODSettings.GetArrayElementAtIndex(i).FindPropertyRelative("height").floatValue;
            }
        }

        private class Styles
        {
            public static GUIContent AlphaTestRef = EditorGUIUtility.TextContent("Alpha Cutoff|The alpha-test reference value.");
            public static GUIContent AnimateCrossFading = EditorGUIUtility.TextContent("Animate Cross-fading|Cross-fading is animated instead of being calculated by distance.");
            public static GUIContent ApplyAndGenerate = EditorGUIUtility.TextContent("Apply & Generate Materials|Apply current importer settings and generate materials with new settings.");
            public static GUIContent CastShadows = EditorGUIUtility.TextContent("Cast Shadows|The tree casts shadow.");
            public static GUIContent CrossFadeWidth = EditorGUIUtility.TextContent("Crossfade Width|Proportion of the last 3D mesh LOD region width which is used for cross-fading to billboard tree.");
            public static GUIContent EnableBump = EditorGUIUtility.TextContent("Normal Map|Enable normal mapping (aka Bump mapping).");
            public static GUIContent EnableHue = EditorGUIUtility.TextContent("Enable Hue Variation|Enable Hue variation color (color is adjusted between Main Color and Hue Color).");
            public static GUIContent FadeOutWidth = EditorGUIUtility.TextContent("Fade Out Width|Proportion of the billboard LOD region width which is used for fading out the billboard.");
            public static GUIContent HueVariation = EditorGUIUtility.TextContent("Hue Color|Apply to LODs that have Hue Variation effect enabled.");
            public static GUIContent LODHeader = EditorGUIUtility.TextContent("LODs");
            public static GUIContent MainColor = EditorGUIUtility.TextContent("Main Color|The color modulating the diffuse lighting component.");
            public static GUIContent MaterialsHeader = EditorGUIUtility.TextContent("Materials");
            public static GUIContent MeshesHeader = EditorGUIUtility.TextContent("Meshes");
            public static GUIContent ReceiveShadows = EditorGUIUtility.TextContent("Receive Shadows|The tree receives shadow.");
            public static GUIContent Regenerate = EditorGUIUtility.TextContent("Regenerate Materials|Regenerate materials from the current importer settings.");
            public static GUIContent ResetLOD = EditorGUIUtility.TextContent("Reset LOD to...|Unify the LOD settings for all selected assets.");
            public static GUIContent ScaleFactor = EditorGUIUtility.TextContent("Scale Factor|How much to scale the tree model compared to what is in the .spm file.");
            public static GUIContent ScaleFactorHelp = EditorGUIUtility.TextContent("The default value of Scale Factor is 0.3048, the conversion ratio from feet to meters, as these are the most conventional measurements used in SpeedTree and Unity, respectively.");
            public static GUIContent Shininess = EditorGUIUtility.TextContent("Shininess|The shininess value.");
            public static GUIContent SmoothLOD = EditorGUIUtility.TextContent("Smooth LOD|Toggles smooth LOD transitions.");
            public static GUIContent SpecColor = EditorGUIUtility.TextContent("Specular Color|The color modulating the specular lighting component.");
            public static GUIContent UseLightProbes = EditorGUIUtility.TextContent("Use Light Probes|The tree uses light probe for lighting.");
            public static GUIContent UseReflectionProbes = EditorGUIUtility.TextContent("Use Reflection Probes|The tree uses reflection probe for rendering.");
            public static GUIContent WindQuality = EditorGUIUtility.TextContent("Wind Quality|Controls the wind quality.");
        }
    }
}


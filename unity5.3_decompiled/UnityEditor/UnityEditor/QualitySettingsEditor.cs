namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [CustomEditor(typeof(QualitySettings))]
    internal class QualitySettingsEditor : Editor
    {
        [CompilerGenerated]
        private static Func<QualitySetting, string> <>f__am$cache8;
        private int m_DeleteLevel = -1;
        private Dragging m_Dragging;
        private SerializedProperty m_PerPlatformDefaultQualityProperty;
        private readonly int m_QualityElementHash = "QualityElementHash".GetHashCode();
        private SerializedObject m_QualitySettings;
        private SerializedProperty m_QualitySettingsProperty;
        private bool m_ShouldAddNewLevel;
        private List<BuildPlayerWindow.BuildPlatform> m_ValidPlatforms;

        private int DoQualityLevelSelection(int currentQualitylevel, IList<QualitySetting> qualitySettings, Dictionary<string, int> platformDefaultQualitySettings)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            int num = currentQualitylevel;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(80f), GUILayout.Height(20f) };
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, options);
            position.x += EditorGUI.indent;
            position.width -= EditorGUI.indent;
            GUI.Label(position, "Levels", EditorStyles.boldLabel);
            foreach (BuildPlayerWindow.BuildPlatform platform in this.m_ValidPlatforms)
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(15f), GUILayout.MaxWidth(20f), GUILayout.Height(20f) };
                Rect rect2 = GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray2);
                GUIContent content = EditorGUIUtility.TempContent(platform.smallIcon);
                content.tooltip = platform.name;
                GUI.Label(rect2, content);
                content.tooltip = string.Empty;
            }
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinWidth(15f), GUILayout.MaxWidth(20f), GUILayout.Height(20f) };
            GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray3);
            GUILayout.EndHorizontal();
            Event current = Event.current;
            for (int i = 0; i < qualitySettings.Count; i++)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUIStyle style = ((i % 2) != 0) ? Styles.kListOddBg : Styles.kListEvenBg;
                bool on = num == i;
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(80f) };
                Rect rect3 = GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray4);
                switch (current.type)
                {
                    case EventType.MouseDown:
                        if (rect3.Contains(current.mousePosition))
                        {
                            num = i;
                            GUIUtility.keyboardControl = 0;
                            GUIUtility.hotControl = this.m_QualityElementHash;
                            Dragging dragging = new Dragging {
                                m_StartPosition = i,
                                m_Position = i
                            };
                            this.m_Dragging = dragging;
                            current.Use();
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == this.m_QualityElementHash)
                        {
                            GUIUtility.hotControl = 0;
                            current.Use();
                        }
                        break;

                    case EventType.MouseDrag:
                        if ((GUIUtility.hotControl == this.m_QualityElementHash) && rect3.Contains(current.mousePosition))
                        {
                            this.m_Dragging.m_Position = i;
                            current.Use();
                        }
                        break;

                    case EventType.KeyDown:
                        if ((current.keyCode == KeyCode.UpArrow) || (current.keyCode == KeyCode.DownArrow))
                        {
                            num += (current.keyCode != KeyCode.UpArrow) ? 1 : -1;
                            num = Mathf.Clamp(num, 0, qualitySettings.Count - 1);
                            GUIUtility.keyboardControl = 0;
                            current.Use();
                        }
                        break;

                    case EventType.Repaint:
                    {
                        style.Draw(rect3, GUIContent.none, false, false, on, false);
                        QualitySetting setting = qualitySettings[i];
                        GUI.Label(rect3, EditorGUIUtility.TempContent(setting.m_Name));
                        break;
                    }
                }
                foreach (BuildPlayerWindow.BuildPlatform platform2 in this.m_ValidPlatforms)
                {
                    bool flag2 = false;
                    if (platformDefaultQualitySettings.ContainsKey(platform2.name) && (platformDefaultQualitySettings[platform2.name] == i))
                    {
                        flag2 = true;
                    }
                    GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.MinWidth(15f), GUILayout.MaxWidth(20f) };
                    Rect rect4 = GUILayoutUtility.GetRect(Styles.kPlatformTooltip, Styles.kToggle, optionArray5);
                    if (Event.current.type == EventType.Repaint)
                    {
                        style.Draw(rect4, GUIContent.none, false, false, on, false);
                    }
                    Color backgroundColor = GUI.backgroundColor;
                    if (flag2 && !EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    QualitySetting setting2 = qualitySettings[i];
                    bool flag3 = !setting2.m_ExcludedPlatforms.Contains(platform2.name);
                    bool flag4 = GUI.Toggle(rect4, flag3, Styles.kPlatformTooltip, !flag2 ? Styles.kToggle : Styles.kDefaultToggle);
                    if (flag3 != flag4)
                    {
                        if (flag4)
                        {
                            QualitySetting setting3 = qualitySettings[i];
                            setting3.m_ExcludedPlatforms.Remove(platform2.name);
                        }
                        else
                        {
                            QualitySetting setting4 = qualitySettings[i];
                            setting4.m_ExcludedPlatforms.Add(platform2.name);
                        }
                    }
                    GUI.backgroundColor = backgroundColor;
                }
                GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.MinWidth(15f), GUILayout.MaxWidth(20f) };
                Rect rect5 = GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray6);
                if (Event.current.type == EventType.Repaint)
                {
                    style.Draw(rect5, GUIContent.none, false, false, on, false);
                }
                if (GUI.Button(rect5, Styles.kIconTrash, GUIStyle.none))
                {
                    this.m_DeleteLevel = i;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray7 = new GUILayoutOption[] { GUILayout.MinWidth(15f), GUILayout.MaxWidth(20f), GUILayout.Height(1f) };
            GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray7);
            DrawHorizontalDivider();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray8 = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(80f), GUILayout.Height(20f) };
            Rect rect6 = GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray8);
            rect6.x += EditorGUI.indent;
            rect6.width -= EditorGUI.indent;
            GUI.Label(rect6, "Default", EditorStyles.boldLabel);
            foreach (BuildPlayerWindow.BuildPlatform platform3 in this.m_ValidPlatforms)
            {
                int num3;
                GUILayoutOption[] optionArray9 = new GUILayoutOption[] { GUILayout.MinWidth(15f), GUILayout.MaxWidth(20f), GUILayout.Height(20f) };
                Rect rect7 = GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray9);
                if (!platformDefaultQualitySettings.TryGetValue(platform3.name, out num3))
                {
                    platformDefaultQualitySettings.Add(platform3.name, 0);
                }
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = x => x.m_Name;
                }
                num3 = EditorGUI.Popup(rect7, num3, qualitySettings.Select<QualitySetting, string>(<>f__am$cache8).ToArray<string>(), Styles.kDefaultDropdown);
                platformDefaultQualitySettings[platform3.name] = num3;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray10 = new GUILayoutOption[] { GUILayout.MinWidth(15f), GUILayout.MaxWidth(20f), GUILayout.Height(20f) };
            GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray10);
            GUILayoutOption[] optionArray11 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            if (GUI.Button(GUILayoutUtility.GetRect(GUIContent.none, Styles.kToggle, optionArray11), EditorGUIUtility.TempContent("Add Quality Level")))
            {
                this.m_ShouldAddNewLevel = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return num;
        }

        private void DrawCascadeSplitGUI<T>(ref SerializedProperty shadowCascadeSplit)
        {
            float[] normalizedCascadePartitions = null;
            Type type = typeof(T);
            if (type == typeof(float))
            {
                normalizedCascadePartitions = new float[] { shadowCascadeSplit.floatValue };
            }
            else if (type == typeof(Vector3))
            {
                Vector3 vector = shadowCascadeSplit.vector3Value;
                normalizedCascadePartitions = new float[] { Mathf.Clamp(vector[0], 0f, 1f), Mathf.Clamp((float) (vector[1] - vector[0]), (float) 0f, (float) 1f), Mathf.Clamp((float) (vector[2] - vector[1]), (float) 0f, (float) 1f) };
            }
            if (normalizedCascadePartitions != null)
            {
                ShadowCascadeSplitGUI.HandleCascadeSliderGUI(ref normalizedCascadePartitions);
                if (type == typeof(float))
                {
                    shadowCascadeSplit.floatValue = normalizedCascadePartitions[0];
                }
                else
                {
                    Vector3 vector2 = new Vector3();
                    vector2[0] = normalizedCascadePartitions[0];
                    vector2[1] = vector2[0] + normalizedCascadePartitions[1];
                    vector2[2] = vector2[1] + normalizedCascadePartitions[2];
                    shadowCascadeSplit.vector3Value = vector2;
                }
            }
        }

        private static void DrawHorizontalDivider()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1f) };
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, options);
            Color backgroundColor = GUI.backgroundColor;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor = (Color) (backgroundColor * 0.7058f);
            }
            else
            {
                GUI.backgroundColor = Color.black;
            }
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUIUtility.whiteTextureStyle.Draw(position, GUIContent.none, false, false, false, false);
            }
            GUI.backgroundColor = backgroundColor;
        }

        private Dictionary<string, int> GetDefaultQualityForPlatforms()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            IEnumerator enumerator = this.m_PerPlatformDefaultQualityProperty.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    SerializedProperty current = (SerializedProperty) enumerator.Current;
                    dictionary.Add(current.FindPropertyRelative("first").stringValue, current.FindPropertyRelative("second").intValue);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return dictionary;
        }

        private List<QualitySetting> GetQualitySettings()
        {
            List<QualitySetting> list = new List<QualitySetting>();
            IEnumerator enumerator = this.m_QualitySettingsProperty.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    SerializedProperty current = (SerializedProperty) enumerator.Current;
                    QualitySetting item = new QualitySetting {
                        m_Name = current.FindPropertyRelative("name").stringValue,
                        m_PropertyPath = current.propertyPath
                    };
                    item.m_PropertyPath = current.propertyPath;
                    List<string> list2 = new List<string>();
                    IEnumerator enumerator2 = current.FindPropertyRelative("excludedTargetPlatforms").GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            SerializedProperty property3 = (SerializedProperty) enumerator2.Current;
                            list2.Add(property3.stringValue);
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator2 as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                    item.m_ExcludedPlatforms = list2;
                    list.Add(item);
                }
            }
            finally
            {
                IDisposable disposable2 = enumerator as IDisposable;
                if (disposable2 == null)
                {
                }
                disposable2.Dispose();
            }
            return list;
        }

        private void HandleAddRemoveQualitySetting(ref int selectedLevel, Dictionary<string, int> platformDefaults)
        {
            if (this.m_DeleteLevel >= 0)
            {
                if ((this.m_DeleteLevel < selectedLevel) || (this.m_DeleteLevel == (this.m_QualitySettingsProperty.arraySize - 1)))
                {
                    selectedLevel--;
                }
                if (((this.m_QualitySettingsProperty.arraySize > 1) && (this.m_DeleteLevel >= 0)) && (this.m_DeleteLevel < this.m_QualitySettingsProperty.arraySize))
                {
                    this.m_QualitySettingsProperty.DeleteArrayElementAtIndex(this.m_DeleteLevel);
                    List<string> list = new List<string>(platformDefaults.Keys);
                    foreach (string str in list)
                    {
                        int num = platformDefaults[str];
                        if ((num != 0) && (num >= this.m_DeleteLevel))
                        {
                            Dictionary<string, int> dictionary;
                            string str2;
                            int num2 = dictionary[str2];
                            (dictionary = platformDefaults)[str2 = str] = num2 - 1;
                        }
                    }
                }
                this.m_DeleteLevel = -1;
            }
            if (this.m_ShouldAddNewLevel)
            {
                this.m_QualitySettingsProperty.arraySize++;
                this.m_QualitySettingsProperty.GetArrayElementAtIndex(this.m_QualitySettingsProperty.arraySize - 1).FindPropertyRelative("name").stringValue = "Level " + (this.m_QualitySettingsProperty.arraySize - 1);
                this.m_ShouldAddNewLevel = false;
            }
        }

        public void OnEnable()
        {
            this.m_QualitySettings = new SerializedObject(this.target);
            this.m_QualitySettingsProperty = this.m_QualitySettings.FindProperty("m_QualitySettings");
            this.m_PerPlatformDefaultQualityProperty = this.m_QualitySettings.FindProperty("m_PerPlatformDefaultQuality");
            this.m_ValidPlatforms = BuildPlayerWindow.GetValidPlatforms();
        }

        public override void OnInspectorGUI()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Changes made in play mode will not be saved.", MessageType.Warning, true);
            }
            this.m_QualitySettings.Update();
            List<QualitySetting> qualitySettings = this.GetQualitySettings();
            Dictionary<string, int> defaultQualityForPlatforms = this.GetDefaultQualityForPlatforms();
            int qualityLevel = QualitySettings.GetQualityLevel();
            qualityLevel = this.DoQualityLevelSelection(qualityLevel, qualitySettings, defaultQualityForPlatforms);
            this.SetQualitySettings(qualitySettings);
            this.HandleAddRemoveQualitySetting(ref qualityLevel, defaultQualityForPlatforms);
            this.SetDefaultQualityForPlatforms(defaultQualityForPlatforms);
            GUILayout.Space(10f);
            DrawHorizontalDivider();
            GUILayout.Space(10f);
            SerializedProperty arrayElementAtIndex = this.m_QualitySettingsProperty.GetArrayElementAtIndex(qualityLevel);
            SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("name");
            SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("pixelLightCount");
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("shadows");
            SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("shadowResolution");
            SerializedProperty property6 = arrayElementAtIndex.FindPropertyRelative("shadowProjection");
            SerializedProperty property7 = arrayElementAtIndex.FindPropertyRelative("shadowCascades");
            SerializedProperty property8 = arrayElementAtIndex.FindPropertyRelative("shadowDistance");
            SerializedProperty property9 = arrayElementAtIndex.FindPropertyRelative("shadowNearPlaneOffset");
            SerializedProperty shadowCascadeSplit = arrayElementAtIndex.FindPropertyRelative("shadowCascade2Split");
            SerializedProperty property11 = arrayElementAtIndex.FindPropertyRelative("shadowCascade4Split");
            SerializedProperty property12 = arrayElementAtIndex.FindPropertyRelative("blendWeights");
            SerializedProperty property13 = arrayElementAtIndex.FindPropertyRelative("textureQuality");
            SerializedProperty property14 = arrayElementAtIndex.FindPropertyRelative("anisotropicTextures");
            SerializedProperty property15 = arrayElementAtIndex.FindPropertyRelative("antiAliasing");
            SerializedProperty property16 = arrayElementAtIndex.FindPropertyRelative("softParticles");
            SerializedProperty property17 = arrayElementAtIndex.FindPropertyRelative("realtimeReflectionProbes");
            SerializedProperty property18 = arrayElementAtIndex.FindPropertyRelative("billboardsFaceCameraPosition");
            SerializedProperty property19 = arrayElementAtIndex.FindPropertyRelative("vSyncCount");
            SerializedProperty property20 = arrayElementAtIndex.FindPropertyRelative("lodBias");
            SerializedProperty property21 = arrayElementAtIndex.FindPropertyRelative("maximumLODLevel");
            SerializedProperty property22 = arrayElementAtIndex.FindPropertyRelative("particleRaycastBudget");
            SerializedProperty property23 = arrayElementAtIndex.FindPropertyRelative("asyncUploadTimeSlice");
            SerializedProperty property24 = arrayElementAtIndex.FindPropertyRelative("asyncUploadBufferSize");
            if (string.IsNullOrEmpty(property.stringValue))
            {
                property.stringValue = "Level " + qualityLevel;
            }
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.Label(EditorGUIUtility.TempContent("Rendering"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property3, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property13, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property14, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property15, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property16, new GUILayoutOption[0]);
            if (property16.boolValue)
            {
                this.SoftParticlesHintGUI();
            }
            EditorGUILayout.PropertyField(property17, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property18, Styles.kBillboardsFaceCameraPos, new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.Label(EditorGUIUtility.TempContent("Shadows"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property4, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property5, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property6, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property8, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property9, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property7, new GUILayoutOption[0]);
            if (property7.intValue == 2)
            {
                this.DrawCascadeSplitGUI<float>(ref shadowCascadeSplit);
            }
            else if (property7.intValue == 4)
            {
                this.DrawCascadeSplitGUI<Vector3>(ref property11);
            }
            GUILayout.Space(10f);
            GUILayout.Label(EditorGUIUtility.TempContent("Other"), EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property12, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property19, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property20, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property21, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property22, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property23, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(property24, new GUILayoutOption[0]);
            if ((this.m_Dragging != null) && (this.m_Dragging.m_Position != this.m_Dragging.m_StartPosition))
            {
                this.m_QualitySettingsProperty.MoveArrayElement(this.m_Dragging.m_StartPosition, this.m_Dragging.m_Position);
                this.m_Dragging.m_StartPosition = this.m_Dragging.m_Position;
                qualityLevel = this.m_Dragging.m_Position;
            }
            this.m_QualitySettings.ApplyModifiedProperties();
            QualitySettings.SetQualityLevel(Mathf.Clamp(qualityLevel, 0, this.m_QualitySettingsProperty.arraySize - 1));
        }

        private void SetDefaultQualityForPlatforms(Dictionary<string, int> platformDefaults)
        {
            if (this.m_PerPlatformDefaultQualityProperty.arraySize != platformDefaults.Count)
            {
                this.m_PerPlatformDefaultQualityProperty.arraySize = platformDefaults.Count;
            }
            int index = 0;
            foreach (KeyValuePair<string, int> pair in platformDefaults)
            {
                SerializedProperty arrayElementAtIndex = this.m_PerPlatformDefaultQualityProperty.GetArrayElementAtIndex(index);
                SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("first");
                SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("second");
                if ((property2.stringValue != pair.Key) || (property3.intValue != pair.Value))
                {
                    property2.stringValue = pair.Key;
                    property3.intValue = pair.Value;
                }
                index++;
            }
        }

        private void SetQualitySettings(IEnumerable<QualitySetting> settings)
        {
            IEnumerator<QualitySetting> enumerator = settings.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    QualitySetting current = enumerator.Current;
                    SerializedProperty property = this.m_QualitySettings.FindProperty(current.m_PropertyPath);
                    if (property != null)
                    {
                        SerializedProperty property2 = property.FindPropertyRelative("excludedTargetPlatforms");
                        if (property2.arraySize != current.m_ExcludedPlatforms.Count)
                        {
                            property2.arraySize = current.m_ExcludedPlatforms.Count;
                        }
                        int num = 0;
                        IEnumerator enumerator2 = property2.GetEnumerator();
                        try
                        {
                            while (enumerator2.MoveNext())
                            {
                                SerializedProperty property3 = (SerializedProperty) enumerator2.Current;
                                if (property3.stringValue != current.m_ExcludedPlatforms[num])
                                {
                                    property3.stringValue = current.m_ExcludedPlatforms[num];
                                }
                                num++;
                            }
                            continue;
                        }
                        finally
                        {
                            IDisposable disposable = enumerator2 as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private void SoftParticlesHintGUI()
        {
            Camera main = Camera.main;
            if (main != null)
            {
                switch (main.actualRenderingPath)
                {
                    case RenderingPath.DeferredLighting:
                    case RenderingPath.DeferredShading:
                        return;
                }
                if ((main.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
                {
                    EditorGUILayout.HelpBox(Styles.kSoftParticlesHint.text, MessageType.Warning, false);
                }
            }
        }

        private class Dragging
        {
            public int m_Position;
            public int m_StartPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct QualitySetting
        {
            public string m_Name;
            public string m_PropertyPath;
            public List<string> m_ExcludedPlatforms;
        }

        private static class Styles
        {
            public static readonly GUIContent kBillboardsFaceCameraPos = EditorGUIUtility.TextContent("Billboards Face Camera Position|Make billboards face towards camera position. Otherwise they face towards camera plane. This makes billboards look nicer when camera rotates but is more expensive to render.");
            public static readonly GUIStyle kButton = "Button";
            public static readonly GUIStyle kDefaultDropdown = "QualitySettingsDefault";
            public static readonly GUIStyle kDefaultToggle = "OL ToggleWhite";
            public const int kHeaderRowHeight = 20;
            public static readonly GUIContent kIconTrash = EditorGUIUtility.IconContent("TreeEditor.Trash", "Delete Level");
            public const int kLabelWidth = 80;
            public static readonly GUIStyle kListEvenBg = "ObjectPickerResultsOdd";
            public static readonly GUIStyle kListOddBg = "ObjectPickerResultsEven";
            public const int kMaxToggleWidth = 20;
            public const int kMinToggleWidth = 15;
            public static readonly GUIContent kPlatformTooltip = new GUIContent(string.Empty, "Allow quality setting on platform");
            public static readonly GUIStyle kSelected = "PR Label";
            public static readonly GUIContent kSoftParticlesHint = EditorGUIUtility.TextContent("Soft Particles require using Deferred Lighting or making camera render the depth texture.");
            public static readonly GUIStyle kToggle = "OL Toggle";
        }
    }
}


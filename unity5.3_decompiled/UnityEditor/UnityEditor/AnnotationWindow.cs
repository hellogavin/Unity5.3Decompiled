namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AnnotationWindow : EditorWindow
    {
        private const float exponentRange = 3f;
        private const float exponentStart = -3f;
        private const float frameWidth = 1f;
        private const float gizmoRightAlign = 23f;
        private GUIContent icon3dGizmoContent = new GUIContent("3D Icons");
        private const float iconRightAlign = 64f;
        private GUIContent iconSelectContent = new GUIContent(string.Empty, "Select Icon");
        private GUIContent iconToggleContent = new GUIContent(string.Empty, "Show/Hide Icon");
        private const float k_AnimDuration = 0.4f;
        private const string kAlwaysFullSizeText = "Always Full Size";
        private const string kHideAllIconsText = "Hide All Icons";
        private const float kWindowWidth = 270f;
        private const float listElementHeight = 18f;
        private List<AInfo> m_BuiltinAnnotations;
        private bool m_IsGameView;
        private string m_LastScriptThatHasShownTheIconSelector;
        private List<MonoScript> m_MonoScriptIconsChanged;
        private List<AInfo> m_RecentAnnotations;
        private List<AInfo> m_ScriptAnnotations;
        private Vector2 m_ScrollPosition;
        private static Styles m_Styles;
        private bool m_SyncWithState;
        private const int maxShowRecent = 5;
        private static AnnotationWindow s_AnnotationWindow;
        private static bool s_Debug;
        private static long s_LastClosedTime;
        private const float scrollBarWidth = 14f;
        private GUIContent showGridContent = new GUIContent("Show Grid");
        private const string textGizmoVisible = "Show/Hide Gizmo";

        private AnnotationWindow()
        {
            base.hideFlags = HideFlags.DontSave;
        }

        private void Cancel()
        {
            base.Close();
            GUI.changed = true;
            GUIUtility.ExitGUI();
        }

        private static float Convert01ToTexelWorldSize(float value01)
        {
            if (value01 <= 0f)
            {
                return 0f;
            }
            return Mathf.Pow(10f, -3f + (3f * value01));
        }

        private static float ConvertTexelWorldSizeTo01(float texelWorldSize)
        {
            if (texelWorldSize == -1f)
            {
                return 1f;
            }
            if (texelWorldSize == 0f)
            {
                return 0f;
            }
            return ((Mathf.Log10(texelWorldSize) - -3f) / 3f);
        }

        private static string ConvertTexelWorldSizeToString(float texelWorldSize)
        {
            if (texelWorldSize == -1f)
            {
                return "Always Full Size";
            }
            if (texelWorldSize == 0f)
            {
                return "Hide All Icons";
            }
            float num = texelWorldSize * 32f;
            int numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference((float) (num * 0.1f));
            return num.ToString("N" + numberOfDecimalsForMinimumDifference);
        }

        private void DrawAnnotationList(float startY, float height)
        {
            Rect position = new Rect(1f, startY + 1f, base.position.width - 2f, (height - 1f) - 1f);
            float num2 = this.DrawNormalList(false, 0f, 0f, 100000f);
            Rect viewRect = new Rect(0f, 0f, 1f, num2);
            bool flag = num2 > position.height;
            float width = position.width;
            if (flag)
            {
                width -= 14f;
            }
            this.m_ScrollPosition = GUI.BeginScrollView(position, this.m_ScrollPosition, viewRect);
            this.DrawNormalList(true, width, this.m_ScrollPosition.y - 18f, this.m_ScrollPosition.y + num2);
            GUI.EndScrollView();
        }

        private void DrawListElement(Rect rect, bool even, AInfo ainfo)
        {
            if (ainfo == null)
            {
                Debug.LogError("DrawListElement: AInfo not valid!");
            }
            else
            {
                string str;
                float width = 17f;
                float a = 0.3f;
                bool changed = GUI.changed;
                bool enabled = GUI.enabled;
                Color color = GUI.color;
                GUI.changed = false;
                GUI.enabled = true;
                GUIStyle style = !even ? m_Styles.listOddBg : m_Styles.listEvenBg;
                GUI.Label(rect, GUIContent.Temp(string.Empty), style);
                Rect position = rect;
                position.width = (rect.width - 64f) - 22f;
                GUI.Label(position, ainfo.m_DisplayText, m_Styles.listTextStyle);
                float num3 = 16f;
                Rect rect3 = new Rect(rect.width - 64f, rect.y + ((rect.height - num3) * 0.5f), num3, num3);
                Texture iconForObject = null;
                if (ainfo.m_ScriptClass != string.Empty)
                {
                    iconForObject = EditorGUIUtility.GetIconForObject(EditorGUIUtility.GetScript(ainfo.m_ScriptClass));
                    Rect rect4 = rect3;
                    rect4.x += 18f;
                    rect4.y++;
                    rect4.width = 1f;
                    rect4.height = 12f;
                    if (!EditorGUIUtility.isProSkin)
                    {
                        GUI.color = new Color(0f, 0f, 0f, 0.33f);
                    }
                    else
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.13f);
                    }
                    GUI.DrawTexture(rect4, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                    GUI.color = Color.white;
                    Rect rect5 = rect3;
                    rect5.x += 18f;
                    rect5.y = rect5.y;
                    rect5.width = 9f;
                    if (GUI.Button(rect5, this.iconSelectContent, m_Styles.iconDropDown))
                    {
                        Object script = EditorGUIUtility.GetScript(ainfo.m_ScriptClass);
                        if (script != null)
                        {
                            this.m_LastScriptThatHasShownTheIconSelector = ainfo.m_ScriptClass;
                            if (IconSelector.ShowAtPosition(script, rect5, true))
                            {
                                IconSelector.SetMonoScriptIconChangedCallback(new IconSelector.MonoScriptIconChangedCallback(this.MonoScriptIconChanged));
                                GUIUtility.ExitGUI();
                            }
                        }
                    }
                }
                else if (ainfo.HasIcon())
                {
                    iconForObject = AssetPreview.GetMiniTypeThumbnailFromClassID(ainfo.m_ClassID);
                }
                if (iconForObject != null)
                {
                    if (!ainfo.m_IconEnabled)
                    {
                        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
                        str = string.Empty;
                    }
                    this.iconToggleContent.image = iconForObject;
                    if (GUI.Button(rect3, this.iconToggleContent, GUIStyle.none))
                    {
                        ainfo.m_IconEnabled = !ainfo.m_IconEnabled;
                        this.SetIconState(ainfo);
                    }
                    GUI.color = color;
                }
                if (GUI.changed)
                {
                    this.SetIconState(ainfo);
                    GUI.changed = false;
                }
                GUI.enabled = true;
                GUI.color = color;
                if (ainfo.HasGizmo())
                {
                    str = "Show/Hide Gizmo";
                    Rect rect6 = new Rect(rect.width - 23f, rect.y + ((rect.height - width) * 0.5f), width, width);
                    ainfo.m_GizmoEnabled = GUI.Toggle(rect6, ainfo.m_GizmoEnabled, new GUIContent(string.Empty, str), m_Styles.toggle);
                    if (GUI.changed)
                    {
                        this.SetGizmoState(ainfo);
                    }
                }
                GUI.enabled = enabled;
                GUI.changed = changed;
                GUI.color = color;
            }
        }

        private void DrawListHeader(string header, Rect rect, ref bool headerDrawn)
        {
            GUI.Label(rect, GUIContent.Temp(header), m_Styles.listHeaderStyle);
            if (!headerDrawn)
            {
                headerDrawn = true;
                GUI.color = new Color(1f, 1f, 1f, 0.65f);
                Rect position = rect;
                position.y += -10f;
                position.x = rect.width - 32f;
                GUI.Label(position, "gizmo", m_Styles.columnHeaderStyle);
                position.x = rect.width - 64f;
                GUI.Label(position, "icon", m_Styles.columnHeaderStyle);
                GUI.color = Color.white;
            }
        }

        private float DrawListSection(float y, string sectionHeader, List<AInfo> listElements, bool doDraw, float listElementWidth, float startY, float endY, ref bool even, bool useSeperator, ref bool headerDrawn)
        {
            float num = y;
            if (listElements.Count <= 0)
            {
                return num;
            }
            if (doDraw)
            {
                Rect position = new Rect(1f, num, listElementWidth - 2f, 30f);
                this.Flip(ref even);
                GUIStyle style = !even ? m_Styles.listOddBg : m_Styles.listEvenBg;
                GUI.Label(position, GUIContent.Temp(string.Empty), style);
            }
            num += 10f;
            if (doDraw)
            {
                this.DrawListHeader(sectionHeader, new Rect(3f, num, listElementWidth, 20f), ref headerDrawn);
            }
            num += 20f;
            for (int i = 0; i < listElements.Count; i++)
            {
                this.Flip(ref even);
                if ((num > startY) && (num < endY))
                {
                    Rect rect = new Rect(1f, num, listElementWidth - 2f, 18f);
                    if (doDraw)
                    {
                        this.DrawListElement(rect, even, listElements[i]);
                    }
                }
                num += 18f;
            }
            if (!useSeperator)
            {
                return num;
            }
            float height = 6f;
            if (doDraw)
            {
                GUIStyle style2 = !even ? m_Styles.listOddBg : m_Styles.listEvenBg;
                GUI.Label(new Rect(1f, num, listElementWidth - 2f, height), GUIContent.Temp(string.Empty), style2);
                GUI.Label(new Rect(10f, num + 3f, listElementWidth - 15f, 3f), GUIContent.Temp(string.Empty), m_Styles.seperator);
            }
            return (num + height);
        }

        private float DrawNormalList(bool doDraw, float listElementWidth, float startY, float endY)
        {
            bool even = true;
            float y = 0f;
            bool headerDrawn = false;
            y = this.DrawListSection(y, "Recently Changed", this.m_RecentAnnotations, doDraw, listElementWidth, startY, endY, ref even, true, ref headerDrawn);
            y = this.DrawListSection(y, "Scripts", this.m_ScriptAnnotations, doDraw, listElementWidth, startY, endY, ref even, false, ref headerDrawn);
            return this.DrawListSection(y, "Built-in Components", this.m_BuiltinAnnotations, doDraw, listElementWidth, startY, endY, ref even, false, ref headerDrawn);
        }

        private void DrawTopSection(float topSectionHeight)
        {
            GUI.Label(new Rect(1f, 0f, base.position.width - 2f, topSectionHeight), string.Empty, EditorStyles.inspectorBig);
            float num = 7f;
            float num2 = 11f;
            float y = num;
            Rect position = new Rect(num2 - 2f, y, 80f, 20f);
            AnnotationUtility.use3dGizmos = GUI.Toggle(position, AnnotationUtility.use3dGizmos, this.icon3dGizmoContent);
            float iconSize = AnnotationUtility.iconSize;
            if (s_Debug)
            {
                Rect rect2 = new Rect(0f, y + 10f, base.position.width - num2, 20f);
                GUI.Label(rect2, ConvertTexelWorldSizeToString(iconSize), m_Styles.texelWorldSizeStyle);
            }
            EditorGUI.BeginDisabledGroup(!AnnotationUtility.use3dGizmos);
            float width = 160f;
            float num6 = ConvertTexelWorldSizeTo01(iconSize);
            Rect rect3 = new Rect((base.position.width - num2) - width, y, width, 20f);
            num6 = GUI.HorizontalSlider(rect3, num6, 0f, 1f);
            if (GUI.changed)
            {
                AnnotationUtility.iconSize = Convert01ToTexelWorldSize(num6);
                SceneView.RepaintAll();
            }
            EditorGUI.EndDisabledGroup();
            y += 20f;
            EditorGUI.BeginDisabledGroup(this.m_IsGameView);
            position = new Rect(num2 - 2f, y, 80f, 20f);
            AnnotationUtility.showGrid = GUI.Toggle(position, AnnotationUtility.showGrid, this.showGridContent);
            EditorGUI.EndDisabledGroup();
        }

        private void Flip(ref bool even)
        {
            even = !even;
        }

        private AInfo GetAInfo(int classID, string scriptClass)
        {
            <GetAInfo>c__AnonStorey48 storey = new <GetAInfo>c__AnonStorey48 {
                scriptClass = scriptClass,
                classID = classID
            };
            if (storey.scriptClass != string.Empty)
            {
                return this.m_ScriptAnnotations.Find(new Predicate<AInfo>(storey.<>m__82));
            }
            return this.m_BuiltinAnnotations.Find(new Predicate<AInfo>(storey.<>m__83));
        }

        private float GetTopSectionHeight()
        {
            return 50f;
        }

        public static void IconChanged()
        {
            if (s_AnnotationWindow != null)
            {
                s_AnnotationWindow.IconHasChanged();
            }
        }

        private void IconHasChanged()
        {
            if (!string.IsNullOrEmpty(this.m_LastScriptThatHasShownTheIconSelector))
            {
                foreach (AInfo info in this.m_ScriptAnnotations)
                {
                    if ((info.m_ScriptClass == this.m_LastScriptThatHasShownTheIconSelector) && !info.m_IconEnabled)
                    {
                        info.m_IconEnabled = true;
                        this.SetIconState(info);
                        break;
                    }
                }
                base.Repaint();
            }
        }

        private void Init(Rect buttonRect, bool isGameView)
        {
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            this.m_MonoScriptIconsChanged = new List<MonoScript>();
            this.m_SyncWithState = true;
            this.m_IsGameView = isGameView;
            this.SyncToState();
            float a = (2f + this.GetTopSectionHeight()) + this.DrawNormalList(false, 100f, 0f, 10000f);
            a = Mathf.Min(a, 900f);
            Vector2 windowSize = new Vector2(270f, a);
            base.ShowAsDropDown(buttonRect, windowSize);
        }

        public void MonoScriptIconChanged(MonoScript monoScript)
        {
            if (monoScript != null)
            {
                bool flag = true;
                foreach (MonoScript script in this.m_MonoScriptIconsChanged)
                {
                    if (script.GetInstanceID() == monoScript.GetInstanceID())
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    this.m_MonoScriptIconsChanged.Add(monoScript);
                }
            }
        }

        private void OnDisable()
        {
            foreach (MonoScript script in this.m_MonoScriptIconsChanged)
            {
                MonoImporter.CopyMonoScriptIconToImporters(script);
            }
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_AnnotationWindow = null;
        }

        private void OnEnable()
        {
        }

        internal void OnGUI()
        {
            if (Event.current.type != EventType.Layout)
            {
                if (m_Styles == null)
                {
                    m_Styles = new Styles();
                }
                if (this.m_SyncWithState)
                {
                    this.SyncToState();
                }
                float topSectionHeight = this.GetTopSectionHeight();
                this.DrawTopSection(topSectionHeight);
                this.DrawAnnotationList(topSectionHeight, base.position.height - topSectionHeight);
                GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, m_Styles.background);
                if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
                {
                    this.Cancel();
                }
            }
        }

        private void SetGizmoState(AInfo ainfo)
        {
            AnnotationUtility.SetGizmoEnabled(ainfo.m_ClassID, ainfo.m_ScriptClass, !ainfo.m_GizmoEnabled ? 0 : 1);
            SceneView.RepaintAll();
        }

        private void SetIconState(AInfo ainfo)
        {
            AnnotationUtility.SetIconEnabled(ainfo.m_ClassID, ainfo.m_ScriptClass, !ainfo.m_IconEnabled ? 0 : 1);
            SceneView.RepaintAll();
        }

        internal static bool ShowAtPosition(Rect buttonRect, bool isGameView)
        {
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num < (s_LastClosedTime + 50L))
            {
                return false;
            }
            Event.current.Use();
            if (s_AnnotationWindow == null)
            {
                s_AnnotationWindow = ScriptableObject.CreateInstance<AnnotationWindow>();
            }
            s_AnnotationWindow.Init(buttonRect, isGameView);
            return true;
        }

        private void SyncToState()
        {
            Annotation[] annotations = AnnotationUtility.GetAnnotations();
            string message = string.Empty;
            if (s_Debug)
            {
                message = message + "AnnotationWindow: SyncToState\n";
            }
            this.m_BuiltinAnnotations = new List<AInfo>();
            this.m_ScriptAnnotations = new List<AInfo>();
            for (int i = 0; i < annotations.Length; i++)
            {
                string str2;
                if (s_Debug)
                {
                    str2 = message;
                    object[] objArray1 = new object[] { str2, "   same as below: icon ", annotations[i].iconEnabled, " gizmo ", annotations[i].gizmoEnabled, "\n" };
                    message = string.Concat(objArray1);
                }
                bool gizmoEnabled = annotations[i].gizmoEnabled == 1;
                bool iconEnabled = annotations[i].iconEnabled == 1;
                AInfo item = new AInfo(gizmoEnabled, iconEnabled, annotations[i].flags, annotations[i].classID, annotations[i].scriptClass);
                if (item.m_ScriptClass == string.Empty)
                {
                    this.m_BuiltinAnnotations.Add(item);
                    if (s_Debug)
                    {
                        str2 = message;
                        object[] objArray2 = new object[] { str2, "   ", BaseObjectTools.ClassIDToString(item.m_ClassID), ": icon ", item.m_IconEnabled, " gizmo ", item.m_GizmoEnabled, "\n" };
                        message = string.Concat(objArray2);
                    }
                }
                else
                {
                    this.m_ScriptAnnotations.Add(item);
                    if (s_Debug)
                    {
                        str2 = message;
                        object[] objArray3 = new object[] { str2, "   ", annotations[i].scriptClass, ": icon ", item.m_IconEnabled, " gizmo ", item.m_GizmoEnabled, "\n" };
                        message = string.Concat(objArray3);
                    }
                }
            }
            this.m_BuiltinAnnotations.Sort();
            this.m_ScriptAnnotations.Sort();
            this.m_RecentAnnotations = new List<AInfo>();
            Annotation[] recentlyChangedAnnotations = AnnotationUtility.GetRecentlyChangedAnnotations();
            for (int j = 0; (j < recentlyChangedAnnotations.Length) && (j < 5); j++)
            {
                AInfo aInfo = this.GetAInfo(recentlyChangedAnnotations[j].classID, recentlyChangedAnnotations[j].scriptClass);
                if (aInfo != null)
                {
                    this.m_RecentAnnotations.Add(aInfo);
                }
            }
            this.m_SyncWithState = false;
            if (s_Debug)
            {
                Debug.Log(message);
            }
        }

        [CompilerGenerated]
        private sealed class <GetAInfo>c__AnonStorey48
        {
            internal int classID;
            internal string scriptClass;

            internal bool <>m__82(AInfo o)
            {
                return (o.m_ScriptClass == this.scriptClass);
            }

            internal bool <>m__83(AInfo o)
            {
                return (o.m_ClassID == this.classID);
            }
        }

        private class Styles
        {
            public GUIStyle background = "grey_border";
            public GUIStyle columnHeaderStyle;
            public GUIStyle iconDropDown = "IN dropdown";
            public GUIStyle listEvenBg = "ObjectPickerResultsOdd";
            public GUIStyle listHeaderStyle;
            public GUIStyle listOddBg = "ObjectPickerResultsEven";
            public GUIStyle listSectionHeaderBg = "ObjectPickerResultsEven";
            public GUIStyle listTextStyle = new GUIStyle(EditorStyles.label);
            public GUIStyle seperator = "sv_iconselector_sep";
            public GUIStyle texelWorldSizeStyle;
            public GUIStyle toggle = "OL Toggle";
            public GUIStyle toolbar = "toolbar";

            public Styles()
            {
                this.listTextStyle.alignment = TextAnchor.MiddleLeft;
                this.listTextStyle.padding.left = 10;
                this.listHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
                this.listHeaderStyle.padding.left = 5;
                this.texelWorldSizeStyle = new GUIStyle(EditorStyles.label);
                this.texelWorldSizeStyle.alignment = TextAnchor.UpperRight;
                this.texelWorldSizeStyle.font = EditorStyles.miniLabel.font;
                this.texelWorldSizeStyle.fontSize = EditorStyles.miniLabel.fontSize;
                this.texelWorldSizeStyle.padding.right = 0;
                this.columnHeaderStyle = new GUIStyle(EditorStyles.miniLabel);
            }
        }
    }
}


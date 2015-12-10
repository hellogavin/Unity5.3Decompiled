namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class LayerVisibilityWindow : EditorWindow
    {
        private const float kFrameWidth = 1f;
        private const string kLayerLocked = "Lock Layer for Picking";
        private const string kLayerVisible = "Show/Hide Layer";
        private const float kScrollBarWidth = 14f;
        private const float kSeparatorHeight = 6f;
        private const float kToggleSize = 17f;
        private int m_AllLayersMask;
        private float m_ContentHeight;
        private Vector2 m_ScrollPosition;
        private static long s_LastClosedTime;
        private List<int> s_LayerMasks;
        private List<string> s_LayerNames;
        private static LayerVisibilityWindow s_LayerVisibilityWindow;
        private static Styles s_Styles;

        private LayerVisibilityWindow()
        {
            base.hideFlags = HideFlags.DontSave;
            base.wantsMouseMove = true;
        }

        private void CalcValidLayers()
        {
            this.s_LayerNames = new List<string>();
            this.s_LayerMasks = new List<int>();
            this.m_AllLayersMask = 0;
            for (int i = 0; i < 0x20; i++)
            {
                string layerName = InternalEditorUtility.GetLayerName(i);
                if (layerName != string.Empty)
                {
                    this.s_LayerNames.Add(layerName);
                    this.s_LayerMasks.Add(i);
                    this.m_AllLayersMask |= ((int) 1) << i;
                }
            }
        }

        private void DoLayerEntry(Rect rect, string layerName, bool even, bool showVisible, bool showLock, bool visible, bool locked, out bool visibleChanged, out bool lockedChanged)
        {
            this.DrawListBackground(rect, even);
            EditorGUI.BeginChangeCheck();
            Rect position = rect;
            position.width -= 34f;
            visible = GUI.Toggle(position, visible, EditorGUIUtility.TempContent(layerName), s_Styles.listTextStyle);
            Rect rect3 = new Rect(rect.width - 34f, rect.y + ((rect.height - 17f) * 0.5f), 17f, 17f);
            visibleChanged = false;
            if (showVisible)
            {
                Color color = GUI.color;
                Color color2 = color;
                color2.a = !visible ? 0.4f : 0.6f;
                GUI.color = color2;
                Rect rect4 = rect3;
                rect4.y += 3f;
                GUIContent content = new GUIContent(string.Empty, !visible ? s_Styles.visibleOff : s_Styles.visibleOn, "Show/Hide Layer");
                GUI.Toggle(rect4, visible, content, GUIStyle.none);
                GUI.color = color;
                visibleChanged = EditorGUI.EndChangeCheck();
            }
            lockedChanged = false;
            if (showLock)
            {
                rect3.x += 17f;
                EditorGUI.BeginChangeCheck();
                Color backgroundColor = GUI.backgroundColor;
                Color color4 = backgroundColor;
                if (!locked)
                {
                    color4.a *= 0.4f;
                }
                GUI.backgroundColor = color4;
                GUI.Toggle(rect3, locked, new GUIContent(string.Empty, "Lock Layer for Picking"), s_Styles.lockButton);
                GUI.backgroundColor = backgroundColor;
                lockedChanged = EditorGUI.EndChangeCheck();
            }
        }

        private void DoOneLayer(Rect rect, int index, ref bool even)
        {
            bool flag3;
            bool flag4;
            int visibleLayers = Tools.visibleLayers;
            int lockedLayers = Tools.lockedLayers;
            int num3 = ((int) 1) << this.s_LayerMasks[index];
            bool visible = (visibleLayers & num3) != 0;
            bool locked = (lockedLayers & num3) != 0;
            this.DoLayerEntry(rect, this.s_LayerNames[index], even, true, true, visible, locked, out flag3, out flag4);
            if (flag3)
            {
                Tools.visibleLayers ^= num3;
                RepaintAllSceneViews();
            }
            if (flag4)
            {
                Tools.lockedLayers ^= num3;
            }
            even = !even;
        }

        private void DoOneSortingLayer(Rect rect, int index, ref bool even)
        {
            bool flag2;
            bool flag3;
            bool sortingLayerLocked = InternalEditorUtility.GetSortingLayerLocked(index);
            this.DoLayerEntry(rect, InternalEditorUtility.GetSortingLayerName(index), even, false, true, true, sortingLayerLocked, out flag2, out flag3);
            if (flag3)
            {
                InternalEditorUtility.SetSortingLayerLocked(index, !sortingLayerLocked);
            }
            even = !even;
        }

        private void DoSpecialLayer(Rect rect, bool all, ref bool even)
        {
            bool flag2;
            bool flag3;
            int visibleLayers = Tools.visibleLayers;
            int num2 = !all ? 0 : this.m_AllLayersMask;
            bool visible = (visibleLayers & this.m_AllLayersMask) == num2;
            this.DoLayerEntry(rect, !all ? "Nothing" : "Everything", even, true, false, visible, false, out flag2, out flag3);
            if (flag2)
            {
                Tools.visibleLayers = !all ? 0 : -1;
                RepaintAllSceneViews();
            }
            even = !even;
        }

        private void Draw(float listElementWidth)
        {
            Rect rect = new Rect(0f, 0f, listElementWidth, 16f);
            bool even = false;
            this.DrawHeader(ref rect, "Layers", ref even);
            this.DoSpecialLayer(rect, true, ref even);
            rect.y += 16f;
            this.DoSpecialLayer(rect, false, ref even);
            rect.y += 16f;
            for (int i = 0; i < this.s_LayerNames.Count; i++)
            {
                this.DoOneLayer(rect, i, ref even);
                rect.y += 16f;
            }
            int sortingLayerCount = InternalEditorUtility.GetSortingLayerCount();
            if (sortingLayerCount > 1)
            {
                this.DrawSeparator(ref rect, even);
                this.DrawHeader(ref rect, "Sorting Layers", ref even);
                for (int j = 0; j < sortingLayerCount; j++)
                {
                    this.DoOneSortingLayer(rect, j, ref even);
                    rect.y += 16f;
                }
            }
            this.DrawSeparator(ref rect, even);
            this.DrawListBackground(rect, even);
            if (GUI.Button(rect, EditorGUIUtility.TempContent("Edit Layers..."), s_Styles.menuItem))
            {
                base.Close();
                Selection.activeObject = EditorApplication.tagManager;
                GUIUtility.ExitGUI();
            }
        }

        private void DrawHeader(ref Rect rect, string text, ref bool even)
        {
            this.DrawListBackground(rect, even);
            GUI.Label(rect, GUIContent.Temp(text), s_Styles.listHeaderStyle);
            rect.y += 16f;
            even = !even;
        }

        private void DrawListBackground(Rect rect, bool even)
        {
            GUIStyle style = !even ? s_Styles.listOddBg : s_Styles.listEvenBg;
            GUI.Label(rect, GUIContent.none, style);
        }

        private void DrawSeparator(ref Rect rect, bool even)
        {
            this.DrawListBackground(new Rect(rect.x + 1f, rect.y, rect.width - 2f, 6f), even);
            GUI.Label(new Rect(rect.x + 5f, rect.y + 3f, rect.width - 10f, 3f), GUIContent.none, s_Styles.separator);
            rect.y += 6f;
        }

        private void Init(Rect buttonRect)
        {
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            this.CalcValidLayers();
            int num = ((this.s_LayerNames.Count + 2) + 1) + 1;
            float a = (num * 16f) + 6f;
            int sortingLayerCount = InternalEditorUtility.GetSortingLayerCount();
            if (sortingLayerCount > 1)
            {
                a += 22f;
                a += sortingLayerCount * 16f;
            }
            this.m_ContentHeight = a;
            a += 2f;
            a = Mathf.Min(a, 600f);
            Vector2 windowSize = new Vector2(180f, a);
            base.ShowAsDropDown(buttonRect, windowSize);
        }

        internal void OnDisable()
        {
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_LayerVisibilityWindow = null;
        }

        internal void OnGUI()
        {
            if (Event.current.type != EventType.Layout)
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                Rect position = new Rect(1f, 1f, base.position.width - 2f, base.position.height - 2f);
                Rect viewRect = new Rect(0f, 0f, 1f, this.m_ContentHeight);
                bool flag = this.m_ContentHeight > position.height;
                float width = position.width;
                if (flag)
                {
                    width -= 14f;
                }
                this.m_ScrollPosition = GUI.BeginScrollView(position, this.m_ScrollPosition, viewRect);
                this.Draw(width);
                GUI.EndScrollView();
                GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, s_Styles.background);
                if (Event.current.type == EventType.MouseMove)
                {
                    Event.current.Use();
                }
                if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
                {
                    base.Close();
                    GUIUtility.ExitGUI();
                }
            }
        }

        private static void RepaintAllSceneViews()
        {
            foreach (SceneView view in Resources.FindObjectsOfTypeAll(typeof(SceneView)))
            {
                view.Repaint();
            }
        }

        internal static bool ShowAtPosition(Rect buttonRect)
        {
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num < (s_LastClosedTime + 50L))
            {
                return false;
            }
            Event.current.Use();
            if (s_LayerVisibilityWindow == null)
            {
                s_LayerVisibilityWindow = ScriptableObject.CreateInstance<LayerVisibilityWindow>();
            }
            s_LayerVisibilityWindow.Init(buttonRect);
            return true;
        }

        private class Styles
        {
            public readonly GUIStyle background = "grey_border";
            public readonly GUIStyle listEvenBg = "ObjectPickerResultsOdd";
            public readonly GUIStyle listHeaderStyle;
            public readonly GUIStyle listOddBg = "ObjectPickerResultsEven";
            public readonly GUIStyle listTextStyle = new GUIStyle(EditorStyles.label);
            public readonly GUIStyle lockButton = "IN LockButton";
            public readonly GUIStyle menuItem = "MenuItem";
            public readonly GUIStyle separator = "sv_iconselector_sep";
            public readonly Texture2D visibleOff = EditorGUIUtility.LoadIcon("animationvisibilitytoggleoff");
            public readonly Texture2D visibleOn = EditorGUIUtility.LoadIcon("animationvisibilitytoggleon");

            public Styles()
            {
                this.listTextStyle.alignment = TextAnchor.MiddleLeft;
                this.listTextStyle.padding.left = 10;
                this.listHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
                this.listHeaderStyle.padding.left = 5;
            }
        }
    }
}


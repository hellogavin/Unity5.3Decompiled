namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class SceneHierarchySortingWindow : EditorWindow
    {
        [CompilerGenerated]
        private static Comparison<InputData> <>f__am$cache5;
        private const float kFrameWidth = 1f;
        private OnSelectCallback m_Callback;
        private List<InputData> m_Data;
        private static long s_LastClosedTime;
        private static SceneHierarchySortingWindow s_SceneHierarchySortingWindow;
        private static Styles s_Styles;

        private SceneHierarchySortingWindow()
        {
            base.hideFlags = HideFlags.DontSave;
            base.wantsMouseMove = true;
        }

        private void Draw()
        {
            Rect rect = new Rect(1f, 1f, base.position.width - 2f, 16f);
            foreach (InputData data in this.m_Data)
            {
                this.DrawListElement(rect, data);
                rect.y += 16f;
            }
        }

        private void DrawListElement(Rect rect, InputData data)
        {
            EditorGUI.BeginChangeCheck();
            GUI.Toggle(rect, data.m_Selected, EditorGUIUtility.TempContent(data.m_Name), s_Styles.menuItem);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Callback(data);
                base.Close();
            }
        }

        private float GetHeight()
        {
            return (16f * this.m_Data.Count);
        }

        private float GetWidth()
        {
            float num = 0f;
            foreach (InputData data in this.m_Data)
            {
                float x = 0f;
                x = s_Styles.menuItem.CalcSize(GUIContent.Temp(data.m_Name)).x;
                if (x > num)
                {
                    num = x;
                }
            }
            return num;
        }

        private void Init(Vector2 pos, List<InputData> data, OnSelectCallback callback)
        {
            Rect guiRect = new Rect(pos.x, pos.y - 16f, 16f, 16f);
            guiRect = GUIUtility.GUIToScreenRect(guiRect);
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = (lhs, rhs) => lhs.m_Name.CompareTo(rhs.m_Name);
            }
            data.Sort(<>f__am$cache5);
            this.m_Data = data;
            this.m_Callback = callback;
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            float y = 2f + this.GetHeight();
            float x = 2f + this.GetWidth();
            Vector2 windowSize = new Vector2(x, y);
            base.ShowAsDropDown(guiRect, windowSize);
        }

        private void OnDisable()
        {
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
        }

        internal void OnGUI()
        {
            if (Event.current.type != EventType.Layout)
            {
                if (Event.current.type == EventType.MouseMove)
                {
                    Event.current.Use();
                }
                this.Draw();
                GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, s_Styles.background);
            }
        }

        internal static bool ShowAtPosition(Vector2 pos, List<InputData> data, OnSelectCallback callback)
        {
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num < (s_LastClosedTime + 50L))
            {
                return false;
            }
            Event.current.Use();
            if (s_SceneHierarchySortingWindow == null)
            {
                s_SceneHierarchySortingWindow = ScriptableObject.CreateInstance<SceneHierarchySortingWindow>();
            }
            s_SceneHierarchySortingWindow.Init(pos, data, callback);
            return true;
        }

        public class InputData
        {
            public string m_Name;
            public bool m_Selected;
            public string m_TypeName;
        }

        public delegate void OnSelectCallback(SceneHierarchySortingWindow.InputData element);

        private class Styles
        {
            public GUIStyle background = "grey_border";
            public GUIStyle menuItem = "MenuItem";
        }
    }
}


namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    public sealed class Highlighter
    {
        private const int kExpansionMovementSize = 5;
        private const float kPopupDuration = 0.33f;
        private const float kPulseSpeed = 0.45f;
        private static float s_HighlightElapsedTime;
        private static GUIStyle s_HighlightStyle;
        private static float s_LastTime;
        private static Rect s_RepaintRegion;
        private static HighlightSearchMode s_SearchMode;
        private static GUIView s_View;

        internal static void ControlHighlightGUI(GUIView self)
        {
            if (((s_View != null) && (self.window == s_View.window)) && (activeVisible && !searching))
            {
                if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "HandleControlHighlight"))
                {
                    if (self.screenPosition.Overlaps(s_RepaintRegion))
                    {
                        self.Repaint();
                    }
                }
                else if (Event.current.type == EventType.Repaint)
                {
                    Rect rect = GUIUtility.ScreenToGUIRect(activeRect);
                    rect = highlightStyle.padding.Add(rect);
                    float num = (Mathf.Cos(((s_HighlightElapsedTime * 3.141593f) * 2f) * 0.45f) + 1f) * 0.5f;
                    float num2 = Mathf.Min((float) 1f, (float) (0.01f + (s_HighlightElapsedTime / 0.33f)));
                    num2 += Mathf.Sin(num2 * 3.141593f) * 0.5f;
                    Vector2 vector = (Vector2) (new Vector2(((rect.width + 5f) / rect.width) - 1f, ((rect.height + 5f) / rect.height) - 1f) * num);
                    Vector2 scale = (Vector2) ((Vector2.one + vector) * num2);
                    Matrix4x4 matrix = GUI.matrix;
                    Color color = GUI.color;
                    GUI.color = new Color(1f, 1f, 1f, 0.8f - (0.3f * num));
                    GUIUtility.ScaleAroundPivot(scale, rect.center);
                    highlightStyle.Draw(rect, false, false, false, false);
                    GUI.color = color;
                    GUI.matrix = matrix;
                }
            }
        }

        internal static void Handle(Rect position, string text)
        {
            INTERNAL_CALL_Handle(ref position, text);
        }

        public static bool Highlight(string windowTitle, string text)
        {
            return Highlight(windowTitle, text, HighlightSearchMode.Auto);
        }

        public static bool Highlight(string windowTitle, string text, HighlightSearchMode mode)
        {
            Stop();
            active = true;
            if (!SetWindow(windowTitle))
            {
                Debug.LogWarning("Window " + windowTitle + " not found.");
                return false;
            }
            activeText = text;
            s_SearchMode = mode;
            s_LastTime = Time.realtimeSinceStartup;
            bool flag = Search();
            if (flag)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(Highlighter.Update));
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(Highlighter.Update));
            }
            else
            {
                Debug.LogWarning("Item " + text + " not found in window " + windowTitle + ".");
                Stop();
            }
            InternalEditorUtility.RepaintAllViews();
            return flag;
        }

        public static void HighlightIdentifier(Rect position, string identifier)
        {
            if ((searchMode == HighlightSearchMode.Identifier) || (searchMode == HighlightSearchMode.Auto))
            {
                Handle(position, identifier);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Handle(ref Rect position, string text);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_internal_get_activeRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_internal_set_activeRect(ref Rect value);
        internal static Rect internal_get_activeRect()
        {
            Rect rect;
            INTERNAL_CALL_internal_get_activeRect(out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string internal_get_activeText();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool internal_get_activeVisible();
        internal static void internal_set_activeRect(Rect value)
        {
            INTERNAL_CALL_internal_set_activeRect(ref value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void internal_set_activeText(string value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void internal_set_activeVisible(bool value);
        private static bool Search()
        {
            searchMode = s_SearchMode;
            s_View.RepaintImmediately();
            if (searchMode == HighlightSearchMode.None)
            {
                return true;
            }
            searchMode = HighlightSearchMode.None;
            Stop();
            return false;
        }

        private static bool SetWindow(string windowTitle)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(GUIView));
            GUIView view = null;
            foreach (GUIView view2 in objArray)
            {
                if (view2 is HostView)
                {
                    if (!((view2 as HostView).actualView.titleContent.text == windowTitle))
                    {
                        continue;
                    }
                    view = view2;
                    break;
                }
                if ((view2.window != null) && (view2.GetType().Name == windowTitle))
                {
                    view = view2;
                    break;
                }
            }
            s_View = view;
            return (view != null);
        }

        public static void Stop()
        {
            active = false;
            activeVisible = false;
            activeText = string.Empty;
            activeRect = new Rect();
            s_LastTime = 0f;
            s_HighlightElapsedTime = 0f;
        }

        private static void Update()
        {
            Rect activeRect = Highlighter.activeRect;
            if ((Highlighter.activeRect.width == 0f) || (s_View == null))
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(Highlighter.Update));
                Stop();
                InternalEditorUtility.RepaintAllViews();
            }
            else
            {
                Search();
            }
            if (activeVisible)
            {
                s_HighlightElapsedTime += Time.realtimeSinceStartup - s_LastTime;
            }
            s_LastTime = Time.realtimeSinceStartup;
            Rect rect = Highlighter.activeRect;
            if (activeRect.width > 0f)
            {
                rect.xMin = Mathf.Min(rect.xMin, activeRect.xMin);
                rect.xMax = Mathf.Max(rect.xMax, activeRect.xMax);
                rect.yMin = Mathf.Min(rect.yMin, activeRect.yMin);
                rect.yMax = Mathf.Max(rect.yMax, activeRect.yMax);
            }
            rect = highlightStyle.padding.Add(rect);
            rect = highlightStyle.overflow.Add(rect);
            rect = new RectOffset(7, 7, 7, 7).Add(rect);
            if (s_HighlightElapsedTime < 0.43f)
            {
                rect = new RectOffset(((int) rect.width) / 2, ((int) rect.width) / 2, ((int) rect.height) / 2, ((int) rect.height) / 2).Add(rect);
            }
            s_RepaintRegion = rect;
            foreach (GUIView view in Resources.FindObjectsOfTypeAll(typeof(GUIView)))
            {
                if (view.window == s_View.window)
                {
                    view.SendEvent(EditorGUIUtility.CommandEvent("HandleControlHighlight"));
                }
            }
        }

        public static bool active
        {
            [CompilerGenerated]
            get
            {
                return <active>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <active>k__BackingField = value;
            }
        }

        public static Rect activeRect
        {
            get
            {
                return internal_get_activeRect();
            }
            private set
            {
                internal_set_activeRect(value);
            }
        }

        public static string activeText
        {
            get
            {
                return internal_get_activeText();
            }
            private set
            {
                internal_set_activeText(value);
            }
        }

        public static bool activeVisible
        {
            get
            {
                return internal_get_activeVisible();
            }
            private set
            {
                internal_set_activeVisible(value);
            }
        }

        private static GUIStyle highlightStyle
        {
            get
            {
                if (s_HighlightStyle == null)
                {
                    s_HighlightStyle = new GUIStyle("ControlHighlight");
                }
                return s_HighlightStyle;
            }
        }

        internal static bool searching { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static HighlightSearchMode searchMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}


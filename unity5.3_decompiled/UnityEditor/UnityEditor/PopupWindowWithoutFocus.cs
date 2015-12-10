namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PopupWindowWithoutFocus : EditorWindow
    {
        private Rect m_ActivatorRect;
        private float m_BorderWidth = 1f;
        private Vector2 m_LastWantedSize = Vector2.zero;
        private PopupLocationHelper.PopupLocation[] m_LocationPriorityOrder;
        private PopupWindowContent m_WindowContent;
        private static Rect s_LastActivatorRect;
        private static double s_LastClosedTime;
        private static PopupWindowWithoutFocus s_PopupWindowWithoutFocus;

        private PopupWindowWithoutFocus()
        {
            base.hideFlags = HideFlags.DontSave;
        }

        private void FitWindowToContent()
        {
            Vector2 windowSize = this.m_WindowContent.GetWindowSize();
            if (this.m_LastWantedSize != windowSize)
            {
                this.m_LastWantedSize = windowSize;
                Vector2 minSize = windowSize + new Vector2(2f * this.m_BorderWidth, 2f * this.m_BorderWidth);
                Rect rect = PopupLocationHelper.GetDropDownRect(this.m_ActivatorRect, minSize, minSize, null, this.m_LocationPriorityOrder);
                base.m_Pos = rect;
                Vector2 vector3 = new Vector2(rect.width, rect.height);
                base.maxSize = vector3;
                base.minSize = vector3;
            }
        }

        public static void Hide()
        {
            if (s_PopupWindowWithoutFocus != null)
            {
                s_PopupWindowWithoutFocus.Close();
            }
        }

        private void Init(Rect activatorRect, PopupWindowContent windowContent, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            this.m_WindowContent = windowContent;
            this.m_WindowContent.editorWindow = this;
            this.m_ActivatorRect = GUIUtility.GUIToScreenRect(activatorRect);
            this.m_LastWantedSize = windowContent.GetWindowSize();
            this.m_LocationPriorityOrder = locationPriorityOrder;
            Vector2 minSize = windowContent.GetWindowSize() + new Vector2(this.m_BorderWidth * 2f, this.m_BorderWidth * 2f);
            base.position = PopupLocationHelper.GetDropDownRect(this.m_ActivatorRect, minSize, minSize, null, this.m_LocationPriorityOrder);
            base.ShowPopup();
            base.Repaint();
        }

        public static bool IsVisible()
        {
            return (s_PopupWindowWithoutFocus != null);
        }

        private void OnDisable()
        {
            s_LastClosedTime = EditorApplication.timeSinceStartup;
            if (this.m_WindowContent != null)
            {
                this.m_WindowContent.OnClose();
            }
            s_PopupWindowWithoutFocus = null;
        }

        private void OnEnable()
        {
            s_PopupWindowWithoutFocus = this;
        }

        private static bool OnGlobalMouseOrKeyEvent(EventType type, KeyCode keyCode, Vector2 mousePosition)
        {
            if (s_PopupWindowWithoutFocus != null)
            {
                if ((type == EventType.KeyDown) && (keyCode == KeyCode.Escape))
                {
                    s_PopupWindowWithoutFocus.Close();
                    return true;
                }
                if ((type == EventType.MouseDown) && !s_PopupWindowWithoutFocus.position.Contains(mousePosition))
                {
                    s_PopupWindowWithoutFocus.Close();
                    return true;
                }
            }
            return false;
        }

        internal void OnGUI()
        {
            this.FitWindowToContent();
            Rect rect = new Rect(this.m_BorderWidth, this.m_BorderWidth, base.position.width - (2f * this.m_BorderWidth), base.position.height - (2f * this.m_BorderWidth));
            this.m_WindowContent.OnGUI(rect);
            GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, "grey_border");
        }

        private static bool ShouldShowWindow(Rect activatorRect)
        {
            if (((EditorApplication.timeSinceStartup - s_LastClosedTime) < 0.2) && !(activatorRect != s_LastActivatorRect))
            {
                return false;
            }
            s_LastActivatorRect = activatorRect;
            return true;
        }

        public static void Show(Rect activatorRect, PopupWindowContent windowContent)
        {
            Show(activatorRect, windowContent, null);
        }

        internal static void Show(Rect activatorRect, PopupWindowContent windowContent, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            if (ShouldShowWindow(activatorRect))
            {
                if (s_PopupWindowWithoutFocus == null)
                {
                    s_PopupWindowWithoutFocus = ScriptableObject.CreateInstance<PopupWindowWithoutFocus>();
                }
                s_PopupWindowWithoutFocus.Init(activatorRect, windowContent, locationPriorityOrder);
            }
        }
    }
}

